using Kalasrapier.Engine.ImportJson;
using OpenTK.Graphics.OpenGL;

namespace Kalasrapier.Engine.Rendering
{
    public class MeshInfo
    {
        // https://www.khronos.org/opengl/wiki/Buffer_Object
        // https://www.khronos.org/opengl/wiki/Vertex_Specification#Vertex_Buffer_Object
        // VBO Handler. VBO is a simple Buffer object, an array of raw data with no aditional
        // information associate to it.
        public int Vbo;

        // Array/struct of metadata, as format references to which VBO is connected
        // https://www.khronos.org/opengl/wiki/Vertex_Specification#Vertex_Array_Object
        // VAO handler.
        public int Vao;

        // IBO
        public int Ibo;

        // Total amount of indices
        public int IndicesLenght = 0;

        // Vertex distribution info
        public VertexInfo VertexInfo { get; set; }

        // Information relate to material slots
        public IndicesPerMaterial[]? Slots;

        // Default Materials applied to the mesh.
        public Material[]? Materials;
        //TODO: Fill those materials


        public void SetActiveMesh()
        {
            GL.BindVertexArray(Vao);
        }

        // TODO: move out
        public void DrawMesh(Shader shader)
        {
            if (Slots is null)
            {
                if (VertexInfo.HasFlag(VertexInfo.UV))
                {
                    // With UV our mesh replicate the vertices making the indexArray useless
                    GL.DrawArrays(PrimitiveType.Triangles, 0, IndicesLenght);
                }
                else
                {
                    // https://docs.gl/gl4/glDrawElements
                    GL.DrawElements(PrimitiveType.Triangles, IndicesLenght, DrawElementsType.UnsignedInt, 0);
                }
            }
            else
            {
                // Make a draw call for each texture color
                for (int i = 0; i < Slots.Length; i++)
                {
                    Materials![i].SetActive(shader);
                    GL.DrawElements(PrimitiveType.Triangles, (int)Slots[i].offset, DrawElementsType.UnsignedInt,
                        (int)(Slots[i].start * sizeof(uint)));
                }
            }

            Utils.CheckGLError("Draw Mesh");
        }

        public void Unload()
        {
            GL.DeleteBuffer(Vbo);
            GL.DeleteBuffer(Ibo);
            GL.DeleteVertexArray(Vao);
        }
    }


    public class MeshManager
    {
        private Dictionary<string, MeshInfo> _meshesInfo = new();

        public Dictionary<string, MeshInfo> MeshesInfo
        {
            get => _meshesInfo;
        }


        public void LoadMeshFormat(string mesh_id, MeshJson meshJson)
        {
            float[] vertexArray;
            uint[] indexArray;
            meshJson.GetVertexArray(out vertexArray);
            meshJson.GetIndexArray(out indexArray);
            LoadMeshDSA(mesh_id, ref vertexArray, ref indexArray, meshJson.GetInfo());
            LoadMaterials(mesh_id, meshJson);
        }

        public void LoadMaterials(string mesh_id, MeshJson meshJson)
        {
            var meshInfo = MeshesInfo[mesh_id];
            meshInfo.Slots = meshJson.index_slots;

            meshInfo.Materials = new Material[meshJson.materials?.Length ?? 0];
            for (int i = 0; i < meshInfo.Materials.Length; i++)
            {
                meshInfo.Materials[i] = new Material(meshJson.materials![i]);
            }
        }

        /// <summary>
        /// Load the mesh throught the DSA extension. https://www.khronos.org/opengl/wiki/Direct_State_Access
        /// This Operation will overwrite the mesh with new data.
        /// </summary>
        public void LoadMeshDSA(string meshId, ref float[] vertexArray, ref uint[] indexArray, VertexInfo info)
        {
            MeshInfo? meshInfo;
            if (!MeshesInfo.TryGetValue(meshId, out meshInfo))
            {
                meshInfo = new MeshInfo();
            }

            GL.CreateBuffers(1, out meshInfo.Vbo);
            Utils.LabelObject(ObjectLabelIdentifier.Buffer, meshInfo.Vbo, "VBO " + meshId);
            // NOTE: glBufferData vs glBufferStorage, the last one specify that the memory size requested wont change on
            // size, in case of changing it again with glBufferStorage, will produce an error.
            // The later also allows to better performance. You can still modify the mapped memory via glSubBufferData*
            // https://docs.gl/gl4/glBufferStorage
            // GL.NamedBufferData(_vertexBufferObject, _meshFormat.vertices.Length * sizeof(float), _meshFormat.vertices, BufferUsageHint.StaticDraw);

            GL.NamedBufferStorage(meshInfo.Vbo, vertexArray.Length * sizeof(float), vertexArray,
                BufferStorageFlags.DynamicStorageBit);
            Utils.CheckGLError("Failed To Load VBO " + meshId);

            GL.CreateVertexArrays(1, out meshInfo.Vao);
            // https://docs.gl/gl4/glBindVertexBuffer
            // https://www.khronos.org/opengl/wiki/Layout_Qualifier_(GLSL)
            // vao, binding index, buffer bind, offset, stride
            // You can bind several vbo to a vao through bindingIndex and bufferHandler
            // TODO
            Utils.LabelObject(ObjectLabelIdentifier.Buffer, meshInfo.Vao, "VAO " + meshId);
            GL.VertexArrayVertexBuffer(meshInfo.Vao, 0, meshInfo.Vbo, 0, info.StrideOffset());
            Utils.CheckGLError("Failed to VAO " + meshId);

            var offsetHelper = VertexInfo.VERTICES;
            var attributeICounter = 0;
            if (!info.HasFlag(VertexInfo.VERTICES))
            {
                throw new Exception("No vertices");
            }

            // https://docs.gl/gl4/glEnableVertexAttribArray
            // Enabled the location 0 on shaders (binding index)
            GL.EnableVertexArrayAttrib(meshInfo.Vao, attributeICounter);
            // https://docs.gl/gl4/glVertexAttribFormat
            // vao, attrib location, length of compounds, type, normalized integer, relative offset
            // Vertices
            GL.VertexArrayAttribFormat(meshInfo.Vao, attributeICounter, VertexInfo.VERTICES.StrideSize(),
                VertexAttribType.Float, false, 0);

            // https://docs.gl/gl4/glVertexAttribBinding
            // vao, attrib index, binding index
            // This allows to connect the attribute index to the binding index, which could be the same VBO or another
            // appart defined in GL.VertexArrayVertexBuffer
            GL.VertexArrayAttribBinding(meshInfo.Vao, attributeICounter, 0);

            if (info.HasFlag(VertexInfo.COLORS))
            {
                GL.EnableVertexArrayAttrib(meshInfo.Vao, attributeICounter);
                GL.VertexArrayAttribFormat(meshInfo.Vao, attributeICounter, VertexInfo.COLORS.StrideSize(),
                    VertexAttribType.Float, false, offsetHelper.StrideOffset());
                GL.VertexArrayAttribBinding(meshInfo.Vao, attributeICounter, 0);
                offsetHelper |= VertexInfo.COLORS;
                attributeICounter++;
            }

            if (info.HasFlag(VertexInfo.UV))
            {
                GL.EnableVertexArrayAttrib(meshInfo.Vao, attributeICounter);
                GL.VertexArrayAttribFormat(meshInfo.Vao, attributeICounter, VertexInfo.UV.StrideSize(),
                    VertexAttribType.Float, false, offsetHelper.StrideOffset());
                GL.VertexArrayAttribBinding(meshInfo.Vao, attributeICounter, 0);
                offsetHelper |= VertexInfo.UV;
                attributeICounter++;
            }

            if (info.HasFlag(VertexInfo.NORMALS))
            {
                GL.EnableVertexArrayAttrib(meshInfo.Vao, attributeICounter);
                GL.VertexArrayAttribFormat(meshInfo.Vao, attributeICounter, VertexInfo.NORMALS.StrideSize(),
                    VertexAttribType.Float, false, offsetHelper.StrideOffset());
                GL.VertexArrayAttribBinding(meshInfo.Vao, attributeICounter, 0);
                offsetHelper |= VertexInfo.NORMALS;
                attributeICounter++;
            }

            if (info.HasFlag(VertexInfo.WEIGHTS))
            {
                GL.EnableVertexArrayAttrib(meshInfo.Vao, attributeICounter);
                GL.VertexArrayAttribFormat(meshInfo.Vao, attributeICounter, VertexInfo.WEIGHTS.StrideSize(),
                    VertexAttribType.Float, false, offsetHelper.StrideOffset());
                GL.VertexArrayAttribBinding(meshInfo.Vao, attributeICounter, 0);
                // offsetHelper |= VertexInfo.WEIGHTS;
                // attributeICounter++;
            }

            GL.CreateBuffers(1, out meshInfo.Ibo);
            GL.NamedBufferStorage(meshInfo.Ibo, indexArray.Length * sizeof(uint), indexArray,
                BufferStorageFlags.DynamicStorageBit);
            Utils.LabelObject(ObjectLabelIdentifier.Buffer, meshInfo.Ibo, "IBO " + meshId);
            // Link the IBO to the VAO
            // https://docs.gl/gl4/glVertexArrayElementBuffer
            GL.VertexArrayElementBuffer(meshInfo.Vao, meshInfo.Ibo);

            _meshesInfo.Add(meshId, meshInfo);
            Utils.CheckGLError("Load Mesh DSA");
        }

        /// <summary>
        /// Load Mesh using the old API. Those require to modify the current opengl context in order to build VAO
        /// and VBO.
        /// </summary>
        // public void LoadMesh()
        // {
        //     _vertexBufferObject = GL.GenBuffer();
        //     GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
        //     GL.BufferData(BufferTarget.ArrayBuffer, _meshFormat.vertices.Length * sizeof(float), _meshFormat.vertices, BufferUsageHint.StaticDraw);
        //
        //     _vertexArrayObject = GL.GenVertexArray();
        //     GL.BindVertexArray(_vertexArrayObject);
        //
        //     GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 7 * sizeof(float), 0); // vertex
        //     GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 7 * sizeof(float), 3 * sizeof(float)); // colors
        //
        //     GL.EnableVertexAttribArray(0);
        //     GL.EnableVertexAttribArray(1);
        //
        //     // The order matters, we need to have create the VAO to make those calls get save into the VAO
        //     _indexBufferObject = GL.GenBuffer();
        //     GL.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBufferObject);
        //     GL.BufferData(BufferTarget.ElementArrayBuffer, _meshFormat.indices.Length * sizeof(uint), _meshFormat.indices, BufferUsageHint.StaticDraw);
        //     // We should unbind at least the elementarraybuffer
        //
        //     Utils.CheckGLError("Load Mesh");
        // }
    }
}