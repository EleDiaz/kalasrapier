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
        public int IndicesLenght;
        // Vertex distribution info
        public VertexInfo VertexInfo { get; set; }
        // Information relate to material slots
        public int[]? Slots;


        public void SetActiveMesh()
        {
            GL.BindVertexArray(Vao);
        }

        // TODO: move out
        public void DrawMesh()
        {
            // https://docs.gl/gl4/glDrawElements
            GL.DrawElements(PrimitiveType.Triangles, IndicesLenght, DrawElementsType.UnsignedInt, 0);
            Utils.CheckGLError("Draw Mesh");
        }

        public void Unload()
        {
            GL.DeleteBuffer(Vbo);
            GL.DeleteBuffer(Ibo);
            GL.DeleteVertexArray(Vao);
        }
    }


    public class Meshes
    {
        private Dictionary<string, MeshInfo> _meshesInfo = new();

        public Dictionary<string, MeshInfo> MeshesInfo { get => _meshesInfo; }

        /// <summary>
        /// Load the mesh throught the DSA extension. https://www.khronos.org/opengl/wiki/Direct_State_Access
        /// This Operation will overwrite the mesh with new data.
        /// </summary>
        public void LoadMeshDSA(string mesh_id, ref float[] vertexArray, ref uint[] indexArray, VertexInfo info)
        {
            var meshInfo = new MeshInfo();
            MeshesInfo.TryGetValue(mesh_id, out meshInfo);

            GL.CreateBuffers(1, out meshInfo.Vbo);
            Utils.LabelObject(ObjectLabelIdentifier.Buffer, meshInfo.Vbo, "VBO " + mesh_id);
            // NOTE: glBufferData vs glBufferStorage, the last one specify that the memory size requested wont change on
            // size, in case of changing it again with glBufferStorage, will produce an error.
            // The later also allows to better performance. You can still modify the mapped memory via glSubBufferData*
            // https://docs.gl/gl4/glBufferStorage
            // GL.NamedBufferData(_vertexBufferObject, _meshFormat.vertices.Length * sizeof(float), _meshFormat.vertices, BufferUsageHint.StaticDraw);

            GL.NamedBufferStorage(meshInfo.Vbo, vertexArray.Length * sizeof(float), vertexArray, BufferStorageFlags.DynamicStorageBit);
            Utils.CheckGLError("Failed To Load VBO " + mesh_id);

            GL.CreateVertexArrays(1, out meshInfo.Vao);
            // https://docs.gl/gl4/glBindVertexBuffer
            // https://www.khronos.org/opengl/wiki/Layout_Qualifier_(GLSL)
            // vao, binding index, buffer bind, offset, stride
            // You can bind several vbo to a vao through bindingIndex and bufferHandler
            // TODO
            Utils.LabelObject(ObjectLabelIdentifier.Buffer, meshInfo.Vao, "VAO " + mesh_id);
            GL.VertexArrayVertexBuffer(meshInfo.Vao, 0, meshInfo.Vbo, 0, info.StrideOffset());
            Utils.CheckGLError("Failed to VAO " + mesh_id);

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
            GL.VertexArrayAttribFormat(meshInfo.Vao, attributeICounter, VertexInfo.VERTICES.StrideSize(), VertexAttribType.Float, false, 0);

            // https://docs.gl/gl4/glVertexAttribBinding
            // vao, attrib index, binding index
            // This allows to connect the attribute index to the binding index, which could be the same VBO or another
            // appart defined in GL.VertexArrayVertexBuffer
            GL.VertexArrayAttribBinding(meshInfo.Vao, attributeICounter, 0);

            if (info.HasFlag(VertexInfo.COLORS))
            {
                GL.EnableVertexArrayAttrib(meshInfo.Vao, attributeICounter);
                GL.VertexArrayAttribFormat(meshInfo.Vao, attributeICounter, VertexInfo.COLORS.StrideSize(), VertexAttribType.Float, false, offsetHelper.StrideOffset());
                GL.VertexArrayAttribBinding(meshInfo.Vao, attributeICounter, 0);
                offsetHelper |= VertexInfo.COLORS;
                attributeICounter++;
            }

            if (info.HasFlag(VertexInfo.UV))
            {
                GL.EnableVertexArrayAttrib(meshInfo.Vao, attributeICounter);
                GL.VertexArrayAttribFormat(meshInfo.Vao, attributeICounter, VertexInfo.UV.StrideSize(), VertexAttribType.Float, false, offsetHelper.StrideOffset());
                GL.VertexArrayAttribBinding(meshInfo.Vao, attributeICounter, 0);
                offsetHelper |= VertexInfo.UV;
                attributeICounter++;
            }

            if (info.HasFlag(VertexInfo.NORMALS))
            {
                GL.EnableVertexArrayAttrib(meshInfo.Vao, attributeICounter);
                GL.VertexArrayAttribFormat(meshInfo.Vao, attributeICounter, VertexInfo.NORMALS.StrideSize(), VertexAttribType.Float, false, offsetHelper.StrideOffset());
                GL.VertexArrayAttribBinding(meshInfo.Vao, attributeICounter, 0);
                offsetHelper |= VertexInfo.NORMALS;
                attributeICounter++;
            }

            if (info.HasFlag(VertexInfo.WEIGHTS))
            {
                GL.EnableVertexArrayAttrib(meshInfo.Vao, attributeICounter);
                GL.VertexArrayAttribFormat(meshInfo.Vao, attributeICounter, VertexInfo.WEIGHTS.StrideSize(), VertexAttribType.Float, false, offsetHelper.StrideOffset());
                GL.VertexArrayAttribBinding(meshInfo.Vao, attributeICounter, 0);
                offsetHelper |= VertexInfo.WEIGHTS;
                attributeICounter++;
            }

            GL.CreateBuffers(1, out meshInfo.Ibo);
            GL.NamedBufferStorage(meshInfo.Ibo, indexArray.Length * sizeof(uint), indexArray, BufferStorageFlags.DynamicStorageBit);
            Utils.LabelObject(ObjectLabelIdentifier.Buffer, meshInfo.Ibo, "IBO " + mesh_id);
            // Link the IBO to the VAO
            // https://docs.gl/gl4/glVertexArrayElementBuffer
            GL.VertexArrayElementBuffer(meshInfo.Vao, meshInfo.Ibo);
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
