using Kalasrapier.Engine.ImportJson;
using Kalasrapier.Engine.Rendering.Components;
using OpenTK.Graphics.OpenGL;

namespace Kalasrapier.Engine.Rendering.Services.MeshManager;

/// <summary>
/// Mesh info contains all handlers associate to the GPU data.
/// </summary>
public class MeshInfo
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
    public Material[]? Materials;
        
    public void SetActiveMesh()
    {
        GL.BindVertexArray(Vao);
    }

    public void Unload()
    {
        GL.DeleteBuffer(Vbo);
        GL.DeleteBuffer(Ibo);
        GL.DeleteVertexArray(Vao);
    }
}