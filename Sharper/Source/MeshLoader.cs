using System.Text.Json;
using OpenTK.Graphics.OpenGL;

namespace Kalasrapier
{
    public class MeshFormat
    {
        public int nvertices { get; set;}
        public float[]? vertices { get; set;}
        public float[]? colors { get; set;}
        public int nindices { get; set;}
        public uint[]? indices { get; set;}
    }

    public class MeshLoader
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

        private MeshFormat _meshFormat;

        public MeshLoader(string meshPath)
        {
            var meshData = File.ReadAllText(meshPath);
            MeshFormat? mesh = JsonSerializer.Deserialize<MeshFormat>(meshData);
            if (mesh == null)
            {
                throw new Exception("Wrong Mesh Format");
            }
            Console.WriteLine(JsonSerializer.Serialize(mesh));

            _meshFormat = mesh;
        }

        
        /// <summary>
        /// Load the mesh throught the DSA extension. https://www.khronos.org/opengl/wiki/Direct_State_Access
        /// </summary>
        public void LoadMeshDSA()
        {
            GL.CreateBuffers(1, out _vertexBufferObject);
            Utils.LabelObject(ObjectLabelIdentifier.Buffer, _vertexBufferObject, "VBO");
            Utils.CheckGLError("Failed to Create Buffers");
            // NOTE: glBufferData vs glBufferStorage, the last one specify that the memory size requeste wont change on
            // size, in case of changing it again with glBufferStorage, will produce an error.
            // The later also allows to better performance. You can still modify the mapped memory via glSubBufferData*
            // https://docs.gl/gl4/glBufferStorage
            // GL.NamedBufferData(_vertexBufferObject, _meshFormat.vertices.Length * sizeof(float), _meshFormat.vertices, BufferUsageHint.StaticDraw);
            GL.NamedBufferStorage(_vertexBufferObject, _meshFormat.vertices.Length * sizeof(float), _meshFormat.vertices, BufferStorageFlags.DynamicStorageBit);
            Utils.CheckGLError("Failed To Load VBO");

            GL.CreateVertexArrays(1, out _vertexArrayObject);
            // https://docs.gl/gl4/glBindVertexBuffer
            // https://www.khronos.org/opengl/wiki/Layout_Qualifier_(GLSL)
            // vao, binding index, buffer bind, offset, stride
            // You can bind several vbo to a vao through bindingIndex and bufferHandler
            GL.VertexArrayVertexBuffer(_vertexArrayObject, 0, _vertexBufferObject, 0, 3 * sizeof(float));
            // https://docs.gl/gl4/glEnableVertexAttribArray
            // Enabled the location 0 on shaders (binding index)
            GL.EnableVertexArrayAttrib(_vertexArrayObject, 0);
            // https://docs.gl/gl4/glVertexAttribFormat
            // vao, attrib location, length of compounds, type, normalized integer, relative offset
            GL.VertexArrayAttribFormat(_vertexArrayObject, 0, 3, VertexAttribType.Float, false, 0);
            // https://docs.gl/gl4/glVertexAttribBinding
            // vao, attrib index, binding index
            // This allows to connect the attribute index to the binding index, which could be the same VBO or another
            // appart defined in GL.VertexArrayVertexBuffer
            GL.VertexArrayAttribBinding(_vertexArrayObject, 0, 0);

            GL.CreateBuffers(1, out _indexBufferObject);
            GL.NamedBufferStorage(_indexBufferObject, _meshFormat.indices.Length * sizeof(uint), _meshFormat.indices, BufferStorageFlags.DynamicStorageBit);
            // Link the IBO to the VAO
            // https://docs.gl/gl4/glVertexArrayElementBuffer
            GL.VertexArrayElementBuffer(_vertexArrayObject, _indexBufferObject);
            Utils.CheckGLError("Load Mesh DSA");
        }

        /// <summary>
        /// Load Mesh using the old API. Those require to modify the current opengl context in order to build VAO
        /// and VBO.
        /// </summary>
        public void LoadMesh()
        {
            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _meshFormat.vertices.Length * sizeof(float), _meshFormat.vertices, BufferUsageHint.StaticDraw);

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // The order matters, we need to have create the VAO to make those calls get save into the VAO
            _indexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _meshFormat.indices.Length * sizeof(uint), _meshFormat.indices, BufferUsageHint.StaticDraw);

            Utils.CheckGLError("Load Mesh");
        }

        public void SetActiveMesh()
        {
            GL.BindVertexArray(_vertexArrayObject);
        }

        public void DrawMesh() {
            SetActiveMesh();
            // https://docs.gl/gl4/glDrawElements
            GL.DrawElements(PrimitiveType.Triangles, _meshFormat.nvertices, DrawElementsType.UnsignedInt, 0);
            Utils.CheckGLError("Draw Mesh");
        }
    }
}
