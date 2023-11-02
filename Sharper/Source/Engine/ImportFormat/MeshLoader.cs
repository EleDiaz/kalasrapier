using System.Text.Json;
using OpenTK.Graphics.OpenGL;

namespace Kalasrapier
{
    /// <summary>
    /// This enum defines the accepted format for each component
    /// </summary>
    [Flags]
    public enum MeshInfo {
        VERTICES = 0,
        COLORS = 1,
        UV = 2,
        NORMALS = 4,
        INDICES = 8,
        WEIGHTS = 16,
    }

    static public class MeshInfoMethods {
        public static int StrideSize(this MeshInfo info) {
            return ComponentSize(MeshInfo.VERTICES) 
                + (info.HasFlag(MeshInfo.COLORS)? ComponentSize(MeshInfo.COLORS): 0)
                + (info.HasFlag(MeshInfo.UV)? ComponentSize(MeshInfo.UV): 0)
                + (info.HasFlag(MeshInfo.NORMALS)? ComponentSize(MeshInfo.NORMALS): 0);
        }

        // TODO: In our implementation all the subcomponents has a size of 4 bytes (floats and uint) for simplicity i
        // will keep like that
        public static int StrideOffset(this MeshInfo info) {
            return info.StrideSize() * sizeof(float);
        }

        /// <summary>
        /// Size on bytes of each component.
        /// </summary>
        public static int ComponentSize(this MeshInfo info) {
            switch (info)
            {
                case MeshInfo.VERTICES:
                    return 3;
                case MeshInfo.COLORS:
                    return 4;
                case MeshInfo.NORMALS:
                    return 3;
                case MeshInfo.UV:
                    return 2;
                default:
                    return 0;
            }
        }
    }


    public class MeshFormat
    {
        // 3 floats
        public float[] vertices { get; set;}
        // 2 floats
        public float[]? uv { get; set;}
        // Colors are assume to be 4 floats
        public float[]? colors { get; set;}
        // 3 floats
        public float[]? normals { get; set;}
        // uints
        public uint[]? indices { get; set;}

        public uint[]? indicesPerMaterial { get; set;}


        public MeshFormat() {
            vertices = new float[0];
        }

        public MeshInfo GetInfo() {
            var flags = MeshInfo.VERTICES;
            if (colors is not null)
                flags |= MeshInfo.COLORS;
            if (uv is not null)
                flags |= MeshInfo.UV;
            if (normals is not null)
                flags |= MeshInfo.NORMALS;
            if (indices is not null)
                flags |= MeshInfo.INDICES;

            return flags;
        }

        // TODO: validate size of each component
        public bool Validate() {
            return true;
        }
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

        // Merge differents buffer into one, order vertex, color
        private float[] mergeBuffers() {
            var temporal = new float[_meshFormat.colors.Length + _meshFormat.vertices.Length];

            Console.WriteLine("colorsL: {0} vertices: {1}", _meshFormat.colors.Length, _meshFormat.vertices.Length);
            int vi = 0;
            int ci = 0;
            for (int i = 0; i < temporal.Length; i++)
            {
                if (i % 7 < 3) {
                    temporal[i] = _meshFormat.vertices[vi];
                    vi++;
                }
                else {
                    temporal[i] = _meshFormat.colors[ci];
                    ci++;
                }
            }
            Console.WriteLine("colorsCI: {0} verticesVI: {1}", ci, vi);

            return temporal;
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
            
            var array = mergeBuffers();

            GL.NamedBufferStorage(_vertexBufferObject, array.Length * sizeof(float), array, BufferStorageFlags.DynamicStorageBit);
            Utils.CheckGLError("Failed To Load VBO");

            GL.CreateVertexArrays(1, out _vertexArrayObject);
            // https://docs.gl/gl4/glBindVertexBuffer
            // https://www.khronos.org/opengl/wiki/Layout_Qualifier_(GLSL)
            // vao, binding index, buffer bind, offset, stride
            // You can bind several vbo to a vao through bindingIndex and bufferHandler
            GL.VertexArrayVertexBuffer(_vertexArrayObject, 0, _vertexBufferObject, 0, 7 * sizeof(float));
            // https://docs.gl/gl4/glEnableVertexAttribArray
            // Enabled the location 0 on shaders (binding index)
            GL.EnableVertexArrayAttrib(_vertexArrayObject, 0);
            GL.EnableVertexArrayAttrib(_vertexArrayObject, 1);
            // https://docs.gl/gl4/glVertexAttribFormat
            // vao, attrib location, length of compounds, type, normalized integer, relative offset
            // Vertices
            GL.VertexArrayAttribFormat(_vertexArrayObject, 0, 3, VertexAttribType.Float, false, 0);

            // Colors
            GL.VertexArrayAttribFormat(_vertexArrayObject, 1, 4, VertexAttribType.Float, false, 3 * sizeof(float));
            // https://docs.gl/gl4/glVertexAttribBinding
            // vao, attrib index, binding index
            // This allows to connect the attribute index to the binding index, which could be the same VBO or another
            // appart defined in GL.VertexArrayVertexBuffer
            GL.VertexArrayAttribBinding(_vertexArrayObject, 0, 0);
            GL.VertexArrayAttribBinding(_vertexArrayObject, 1, 0);

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

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 7 * sizeof(float), 0); // vertex
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 7 * sizeof(float), 3 * sizeof(float)); // colors

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);

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
            // SetActiveMesh();
            // https://docs.gl/gl4/glDrawElements
            GL.DrawElements(PrimitiveType.Triangles, _meshFormat.indices.Length, DrawElementsType.UnsignedInt, 0);
            Utils.CheckGLError("Draw Mesh");
        }

        public void Unload() {
            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteBuffer(_indexBufferObject);
            GL.DeleteVertexArray(_vertexArrayObject);
        }
    }
}
