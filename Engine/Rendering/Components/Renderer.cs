using Kalasrapier.Engine.ImportJson;
using Kalasrapier.Engine.Rendering.Actors;
using Kalasrapier.Engine.Rendering.Services.MeshManager;
using OpenTK.Graphics.OpenGL;

namespace Kalasrapier.Engine.Rendering.Components;

public class RendererMesh : Component
{
    // https://www.khronos.org/opengl/wiki/Buffer_Object
    // https://www.khronos.org/opengl/wiki/Vertex_Specification#Vertex_Buffer_Object
    // VBO Handler. VBO is a simple Buffer object, an array of raw data with no additional
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
    public IndicesPerMaterialData[]? Slots;

    // Default Materials applied to the mesh.
    // TODO: remove
    public Material[]? Materials;

    // for debugging
    private string _meshId { get; set; } = "";


    public RendererMesh(Actor actor) : base(actor)
    {
        _meshId = actor.GetComponent<Mesh>()?.MeshId ?? "NO_MESH_ASSOCIATE";
    }

    /// <summary>
    /// Load the mesh through the DSA extension. https://www.khronos.org/opengl/wiki/Direct_State_Access
    /// This Operation will overwrite the mesh with new data.
    /// </summary>
    public void LoadMeshDSA(ref float[] vertexArray, ref uint[] indexArray, VertexInfo info)
    {

        GL.CreateBuffers(1, out Vbo);
        Utils.LabelObject(ObjectLabelIdentifier.Buffer, Vbo, "VBO ");
        // NOTE: glBufferData vs glBufferStorage, the last one specify that the memory size requested wont change on
        // size, in case of changing it again with glBufferStorage, will produce an error.
        // The later also allows to better performance. You can still modify the mapped memory via glSubBufferData*
        // https://docs.gl/gl4/glBufferStorage
        // GL.NamedBufferData(_vertexBufferObject, _meshFormat.vertices.Length * sizeof(float), _meshFormat.vertices, BufferUsageHint.StaticDraw);

        GL.NamedBufferStorage(Vbo, vertexArray.Length * sizeof(float), vertexArray,
            BufferStorageFlags.DynamicStorageBit);
        Utils.CheckGlError("Failed To Load VBO " + _meshId);

        GL.CreateVertexArrays(1, out Vao);
        // https://docs.gl/gl4/glBindVertexBuffer
        // https://www.khronos.org/opengl/wiki/Layout_Qualifier_(GLSL)
        // vao, binding index, buffer bind, offset, stride
        // You can bind several vbo to a vao through bindingIndex and bufferHandler
        // TODO
        Utils.LabelObject(ObjectLabelIdentifier.Buffer, Vao, "VAO " + _meshId);
        GL.VertexArrayVertexBuffer(Vao, 0, Vbo, 0, info.StrideOffset());
        Utils.CheckGlError("Failed to VAO " + _meshId);

        var offsetHelper = VertexInfo.VERTICES;
        var attributeICounter = 0;
        if (!info.HasFlag(VertexInfo.VERTICES))
        {
            throw new Exception("No vertices");
        }

        // https://docs.gl/gl4/glEnableVertexAttribArray
        // Enabled the location 0 on shaders (binding index)
        GL.EnableVertexArrayAttrib(Vao, attributeICounter);
        // https://docs.gl/gl4/glVertexAttribFormat
        // vao, attrib location, length of compounds, type, normalized integer, relative offset
        // Vertices
        GL.VertexArrayAttribFormat(Vao, attributeICounter, VertexInfo.VERTICES.StrideSize(),
            VertexAttribType.Float, false, 0);

        // https://docs.gl/gl4/glVertexAttribBinding
        // vao, attrib index, binding index
        // This allows to connect the attribute index to the binding index, which could be the same VBO or another
        // appart defined in GL.VertexArrayVertexBuffer
        GL.VertexArrayAttribBinding(Vao, attributeICounter, 0);

        if (info.HasFlag(VertexInfo.COLORS))
        {
            GL.EnableVertexArrayAttrib(Vao, attributeICounter);
            GL.VertexArrayAttribFormat(Vao, attributeICounter, VertexInfo.COLORS.StrideSize(),
                VertexAttribType.Float, false, offsetHelper.StrideOffset());
            GL.VertexArrayAttribBinding(Vao, attributeICounter, 0);
            offsetHelper |= VertexInfo.COLORS;
            attributeICounter++;
        }

        if (info.HasFlag(VertexInfo.UV))
        {
            GL.EnableVertexArrayAttrib(Vao, attributeICounter);
            GL.VertexArrayAttribFormat(Vao, attributeICounter, VertexInfo.UV.StrideSize(),
                VertexAttribType.Float, false, offsetHelper.StrideOffset());
            GL.VertexArrayAttribBinding(Vao, attributeICounter, 0);
            offsetHelper |= VertexInfo.UV;
            attributeICounter++;
        }

        if (info.HasFlag(VertexInfo.NORMALS))
        {
            GL.EnableVertexArrayAttrib(Vao, attributeICounter);
            GL.VertexArrayAttribFormat(Vao, attributeICounter, VertexInfo.NORMALS.StrideSize(),
                VertexAttribType.Float, false, offsetHelper.StrideOffset());
            GL.VertexArrayAttribBinding(Vao, attributeICounter, 0);
            offsetHelper |= VertexInfo.NORMALS;
            attributeICounter++;
        }

        if (info.HasFlag(VertexInfo.WEIGHTS))
        {
            GL.EnableVertexArrayAttrib(Vao, attributeICounter);
            GL.VertexArrayAttribFormat(Vao, attributeICounter, VertexInfo.WEIGHTS.StrideSize(),
                VertexAttribType.Float, false, offsetHelper.StrideOffset());
            GL.VertexArrayAttribBinding(Vao, attributeICounter, 0);
            // offsetHelper |= VertexInfo.WEIGHTS;
            // attributeICounter++;
        }

        GL.CreateBuffers(1, out Ibo);
        GL.NamedBufferStorage(Ibo, indexArray.Length * sizeof(uint), indexArray,
            BufferStorageFlags.DynamicStorageBit);
        Utils.LabelObject(ObjectLabelIdentifier.Buffer, Ibo, "IBO " + _meshId);
        // Link the IBO to the VAO
        // https://docs.gl/gl4/glVertexArrayElementBuffer
        GL.VertexArrayElementBuffer(Vao, Ibo);

        Utils.CheckGlError("Load Mesh DSA");
    }


    public void SetActiveMesh()
    {
        GL.BindVertexArray(Vao);
    }

    public override void Destroy()
    {
        GL.DeleteBuffer(Vbo);
        GL.DeleteBuffer(Ibo);
        GL.DeleteVertexArray(Vao);
    }
}