
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Kalasrapier
{
    // TODO: This is the simplest idea to make object to be draw. With multiple instances, i should recommend to implement
    // its own version, which would share VAO/EBO, and other attributes.
    public class Object
    {
        // https://www.khronos.org/opengl/wiki/Buffer_Object
        // https://www.khronos.org/opengl/wiki/Vertex_Specification#Vertex_Buffer_Object
        // VBO Handler. VBO is a simple Buffer object, an array of raw data with no aditional
        // information associate to it.
        private int _vertexBufferObject;
        // Array/struct of metadata, as format references to which VBO is connected
        // https://www.khronos.org/opengl/wiki/Vertex_Specification#Vertex_Array_Object
        // VAO handler.
        private int _vertexArrayObject;

        // IBO
        private int _indexBufferObject;

        private int _indexLenght;

        private MeshInfo _meshInfo;

        private Matrix4 _model;

        public int IndexLength { get => _indexLenght; set => _indexLenght = value; }

        /// <summary>
        /// Load the mesh throught the DSA extension. https://www.khronos.org/opengl/wiki/Direct_State_Access
        /// </summary>
        public void LoadMeshDSA(ref float[] vertexArray, ref uint[] indexArray, MeshInfo info)
        {
            GL.CreateBuffers(1, out _vertexBufferObject);
            Utils.LabelObject(ObjectLabelIdentifier.Buffer, _vertexBufferObject, "VBO");
            Utils.CheckGLError("Failed to Create Buffers");
            // NOTE: glBufferData vs glBufferStorage, the last one specify that the memory size requested wont change on
            // size, in case of changing it again with glBufferStorage, will produce an error.
            // The later also allows to better performance. You can still modify the mapped memory via glSubBufferData*
            // https://docs.gl/gl4/glBufferStorage
            // GL.NamedBufferData(_vertexBufferObject, _meshFormat.vertices.Length * sizeof(float), _meshFormat.vertices, BufferUsageHint.StaticDraw);

            GL.NamedBufferStorage(_vertexBufferObject, vertexArray.Length * sizeof(float), vertexArray, BufferStorageFlags.DynamicStorageBit);
            Utils.CheckGLError("Failed To Load VBO");

            GL.CreateVertexArrays(1, out _vertexArrayObject);
            // https://docs.gl/gl4/glBindVertexBuffer
            // https://www.khronos.org/opengl/wiki/Layout_Qualifier_(GLSL)
            // vao, binding index, buffer bind, offset, stride
            // You can bind several vbo to a vao through bindingIndex and bufferHandler
            // TODO
            GL.VertexArrayVertexBuffer(_vertexArrayObject, 0, _vertexBufferObject, 0, info.StrideOffset());

            var offsetHelper = MeshInfo.VERTICES;
            var attributeICounter = 0;
            if (!info.HasFlag(MeshInfo.VERTICES))
            {
                throw new Exception("No vertices");
            }
            // https://docs.gl/gl4/glEnableVertexAttribArray
            // Enabled the location 0 on shaders (binding index)
            GL.EnableVertexArrayAttrib(_vertexArrayObject, attributeICounter);
            // https://docs.gl/gl4/glVertexAttribFormat
            // vao, attrib location, length of compounds, type, normalized integer, relative offset
            // Vertices
            GL.VertexArrayAttribFormat(_vertexArrayObject, attributeICounter, MeshInfo.VERTICES.StrideSize(), VertexAttribType.Float, false, 0);

            // https://docs.gl/gl4/glVertexAttribBinding
            // vao, attrib index, binding index
            // This allows to connect the attribute index to the binding index, which could be the same VBO or another
            // appart defined in GL.VertexArrayVertexBuffer
            GL.VertexArrayAttribBinding(_vertexArrayObject, attributeICounter, 0);

            if (info.HasFlag(MeshInfo.COLORS))
            {
                GL.EnableVertexArrayAttrib(_vertexArrayObject, attributeICounter);
                GL.VertexArrayAttribFormat(_vertexArrayObject, attributeICounter, MeshInfo.COLORS.StrideSize(), VertexAttribType.Float, false, offsetHelper.StrideOffset());
                GL.VertexArrayAttribBinding(_vertexArrayObject, attributeICounter, 0);
                offsetHelper |= MeshInfo.COLORS;
                attributeICounter++;
            }

            if (info.HasFlag(MeshInfo.UV)) {
                GL.EnableVertexArrayAttrib(_vertexArrayObject, attributeICounter);
                GL.VertexArrayAttribFormat(_vertexArrayObject, attributeICounter, MeshInfo.UV.StrideSize(), VertexAttribType.Float, false, offsetHelper.StrideOffset());
                GL.VertexArrayAttribBinding(_vertexArrayObject, attributeICounter, 0);
                offsetHelper |= MeshInfo.UV;
                attributeICounter++;
            }

            if (info.HasFlag(MeshInfo.NORMALS)) {
                GL.EnableVertexArrayAttrib(_vertexArrayObject, attributeICounter);
                GL.VertexArrayAttribFormat(_vertexArrayObject, attributeICounter, MeshInfo.NORMALS.StrideSize(), VertexAttribType.Float, false, offsetHelper.StrideOffset());
                GL.VertexArrayAttribBinding(_vertexArrayObject, attributeICounter, 0);
                offsetHelper |= MeshInfo.NORMALS;
                attributeICounter++;
            }

            GL.CreateBuffers(1, out _indexBufferObject);
            GL.NamedBufferStorage(_indexBufferObject, indexArray.Length * sizeof(uint), indexArray, BufferStorageFlags.DynamicStorageBit);
            // Link the IBO to the VAO
            // https://docs.gl/gl4/glVertexArrayElementBuffer
            GL.VertexArrayElementBuffer(_vertexArrayObject, _indexBufferObject);
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
        //
        //     Utils.CheckGLError("Load Mesh");
        // }


        public void SetActiveMesh()
        {
            GL.BindVertexArray(_vertexArrayObject);
        }

        public void Update(float deltaTime) {
            
        }

        public void DrawMesh()
        {
            // SetActiveMesh();
            // https://docs.gl/gl4/glDrawElements
            GL.DrawElements(PrimitiveType.Triangles, IndexLength, DrawElementsType.UnsignedInt, 0);
            Utils.CheckGLError("Draw Mesh");
        }

        public void Unload()
        {
            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteBuffer(_indexBufferObject);
            GL.DeleteVertexArray(_vertexArrayObject);
        }
    }

    public class World
    {
        // TODO: change Object class to an interface
        private List<Object> _objects;

        private Level _level;

        public World()
        {
            _objects = new List<Object>();
            _level = new Level();
        }

        // Para el tema de los materiales aquí quizás haya que tocar el Index. Y alguna info addicional de 
        // los materiales
        public void InitLevel()
        {
            float[] vertexArray;
            uint[] indexArray;
            for (int i = 0; i < _level.AmountOfMeshes(); i++)
            {
                _level.GetVertexArray(i, out vertexArray);
                _level.GetIndexArray(i, out indexArray);
                var obj = new Object();
                obj.LoadMeshDSA(ref vertexArray, ref indexArray, _level.GetInfo(i));
                obj.IndexLength = indexArray.Length;
                _objects.Add(obj);
            }
        }

        public void Update(float deltaTime) {
            _objects.ForEach(obj => Update(deltaTime));
        }
    }
}
