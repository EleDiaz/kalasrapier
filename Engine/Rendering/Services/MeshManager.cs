using System.Text.Json;
using Kalasrapier.Engine.ImportJson;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Kalasrapier.Engine.Rendering.Services;

/// <summary>
/// This enum defines the accepted format for each component
/// </summary>
[Flags]
public enum VertexInfo
{
    VERTICES = 1,
    COLORS = 2,
    UV = 4,
    NORMALS = 8,
    WEIGHTS = 16,
}

public static class VertexInfoMethods
{
    public static int StrideSize(this VertexInfo info)
    {
        return ComponentSize(VertexInfo.VERTICES)
               + (info.HasFlag(VertexInfo.COLORS) ? ComponentSize(VertexInfo.COLORS) : 0)
               + (info.HasFlag(VertexInfo.UV) ? ComponentSize(VertexInfo.UV) : 0)
               + (info.HasFlag(VertexInfo.NORMALS) ? ComponentSize(VertexInfo.NORMALS) : 0)
               + (info.HasFlag(VertexInfo.WEIGHTS) ? ComponentSize(VertexInfo.WEIGHTS) : 0);
    }

    // TODO: In our implementation all the subcomponents has a size of 4 bytes (floats and uint) for simplicity i
    //       will keep it like that
    public static int StrideOffset(this VertexInfo info)
    {
        return info.StrideSize() * sizeof(float);
    }

    /// <summary>
    /// Size on bytes of each component.
    /// </summary>
    public static int ComponentSize(this VertexInfo info)
    {
        switch (info)
        {
            case VertexInfo.VERTICES:
                return 3;
            case VertexInfo.COLORS:
                return 4;
            case VertexInfo.NORMALS:
                return 3;
            case VertexInfo.UV:
                return 2;
            case VertexInfo.WEIGHTS:
                return 1;
            default:
                return 0;
        }
    }
}
    
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


public class MeshManager : Base
{
    private Dictionary<string, MeshData> _meshesJson = new();

    public Dictionary<string, MeshInfo> MeshesInfo { get; } = new();

    /// <summary>
    /// Add json mesh to the manager, so it can be loaded later to the gpu.
    /// </summary>
    /// <param name="file"></param>
    /// <param name="id"></param>
    /// <exception cref="Exception"></exception>
    public void AddMeshResource(string file, string id)
    {
        var meshData = File.ReadAllText(file);

        var mesh = JsonSerializer.Deserialize<MeshData>(meshData);
        if (mesh is null)
        {
            throw new Exception(String.Format("Mesh is null file: {0}, id: {1}", file, id));
        }

        _meshesJson.Add(id, mesh);
    }

    public void LoadMesh(string mesh_id, VertexInfo info)
    {
        var meshJson = _meshesJson[mesh_id];
        float[] vertexArray;
        uint[] indexArray;
        GetVertexArray(meshJson, out vertexArray, info);
        GetIndexArray(meshJson, out indexArray);
        LoadMeshDSA(mesh_id, ref vertexArray, ref indexArray, info);
    }

    public void LoadMaterials(string mesh_id, MeshData meshData)
    {
        var meshInfo = MeshesInfo[mesh_id];
        meshInfo.Slots = meshData.IndexSlots;

        meshInfo.Materials = new Material[meshData.Materials?.Length ?? 0];
        for (int i = 0; i < meshInfo.Materials.Length; i++)
        {
            meshInfo.Materials[i] = new Material(meshData.Materials![i]);
        }
    }

    /// <summary>
    /// Load the mesh through the DSA extension. https://www.khronos.org/opengl/wiki/Direct_State_Access
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
        Utils.CheckGlError("Failed To Load VBO " + meshId);

        GL.CreateVertexArrays(1, out meshInfo.Vao);
        // https://docs.gl/gl4/glBindVertexBuffer
        // https://www.khronos.org/opengl/wiki/Layout_Qualifier_(GLSL)
        // vao, binding index, buffer bind, offset, stride
        // You can bind several vbo to a vao through bindingIndex and bufferHandler
        // TODO
        Utils.LabelObject(ObjectLabelIdentifier.Buffer, meshInfo.Vao, "VAO " + meshId);
        GL.VertexArrayVertexBuffer(meshInfo.Vao, 0, meshInfo.Vbo, 0, info.StrideOffset());
        Utils.CheckGlError("Failed to VAO " + meshId);

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

        MeshesInfo.Add(meshId, meshInfo);
        Utils.CheckGlError("Load Mesh DSA");
    }

        
    public VertexInfo GetInfo(MeshData meshData)
    {
        var flags = VertexInfo.VERTICES;
        if (meshData.ColorData is not null)
            flags |= VertexInfo.COLORS;
        if (meshData.UvData is not null)
            flags |= VertexInfo.UV;
        if (meshData.NormalData is not null)
            flags |= VertexInfo.NORMALS;
        if (meshData.WeightData is not null)
            flags |= VertexInfo.WEIGHTS;

        return flags;
    }

    // TODO: validate size of each component
    public bool Validate(MeshData meshData)
    {
            
        return true;
    }

    public void GetVertexArray(MeshData meshData, out float[] vertexData, VertexInfo info)
    {
        // We can't directly use the vertex length when we work with UVs. Due, that each UV is linked to a vertex
        // inside a triangle primitive and those primitive could have those vertices 0 or more times shared with others.
        // Also, one would be encouraged to think that the UV points would match the index points. But it is false
        // depending on the representation of those index points. We will be using one where the indexes aren't
        // reuse between triangles. So, our UVs will always be double size of our index buffer.
        //
        // The code is implemented to allow the case where the vertices, normals, colors... are associate to the
        // each polygon primitive.

        var getSize = (VertexInfo info) => {
            return meshData.UvData?.Length / 2 * info.ComponentSize();
        };

        var verticesLength = getSize(VertexInfo.VERTICES) ?? meshData.VertexData.Length;
        // Normals are associate to a vertex 1-1. This could change be to achieve a Flat Shading where each triangle
        // would share the face normal.
        var normalLength = getSize(VertexInfo.NORMALS) ?? meshData.NormalData?.Length ?? 0;
        // Colors are associate to a vertex 1-1.
        var colorsLength = getSize(VertexInfo.COLORS) ?? meshData.ColorData?.Length ?? 0;
        // Weights are associate to a vertex 1-1.
        var weightsLength = getSize(VertexInfo.COLORS) ?? meshData.WeightData?.Length ?? 0;

        var uvLength = meshData.UvData?.Length ?? 0;

        var size = verticesLength + normalLength + colorsLength + weightsLength + uvLength;

        var strideSize = info.StrideSize();
            
        var getOffset = (int currentOffset, VertexInfo component) => {
            return currentOffset + (info.HasFlag(component) ? component.ComponentSize() : 0);
        };

        vertexData = new float[size];
        int vI = 0;
        var vertexOffset = getOffset(0, VertexInfo.VERTICES);
        int cI = 0;
        var colorOffset = getOffset(vertexOffset, VertexInfo.COLORS);
        int uvI = 0;
        var uvOffset = getOffset(colorOffset, VertexInfo.UV);
        int nI = 0;
        var normalOffset = getOffset(uvOffset, VertexInfo.NORMALS);
        int wI = 0;
        var weightsOffset = getOffset(normalOffset, VertexInfo.WEIGHTS);


        var fillValuesWithIndices = (ref float[] vertexData, ref int ix, ref int index, float[] array, VertexInfo component) => {
            // When UV is active the index will be associate to Index the indices.
            if (info.HasFlag(VertexInfo.UV))
            {
                for (int j = 0; j < component.ComponentSize(); j++)
                {
                    vertexData[ix++] = array[3 * meshData.IndexData![index] + j];
                }
                ix--;
            }
            else
            {
                vertexData[ix] = array[index];
            }
            index++;
        };

        for (int i = 0; i < vertexData.Length; i++)
        {
            if (i % strideSize < vertexOffset)
            {
                fillValuesWithIndices(ref vertexData, ref i, ref vI, meshData.VertexData, VertexInfo.VERTICES);
            }
            else if (i % strideSize < colorOffset)
            {
                fillValuesWithIndices(ref vertexData, ref i, ref cI, meshData.ColorData!, VertexInfo.COLORS);
            }
            else if (i % strideSize < uvOffset)
            {
                vertexData[i] = meshData.UvData![uvI];
                uvI++;
            }
            else if (i % strideSize < normalOffset)
            {
                fillValuesWithIndices(ref vertexData, ref i, ref nI, meshData.NormalData!, VertexInfo.NORMALS);
            }
            else if (i % strideSize < weightsOffset)
            {
                fillValuesWithIndices(ref vertexData, ref i, ref wI, meshData.WeightData!, VertexInfo.WEIGHTS);
            }
        }
    }

    // Drawing by index comes with performance penalty. Unless our target machine is memory limited
    // or we need the use of several index buffers to render different parts of our mesh. The index buffer will
    // only produce a penalty due the simple fact is require a prefetch of those vertices. Where the simple method
    // DrawArrays benefits from data location.
    public void GetIndexArray(MeshData meshData, out uint[] indexArray)
    {
        if (meshData.IndexData is null)
        {
            throw new Exception("Mesh didn't come with indices");
        }

        indexArray = new uint[meshData.IndexData!.Length];
        meshData.IndexData!.CopyTo(indexArray, 0);
    }

    public List<Vector3> GetVertexList(string? actorMeshId)
    {
        if (actorMeshId is null)
        {
            return new List<Vector3>();
        }

        var vertexData = _meshesJson[actorMeshId].VertexData;
            
        if (vertexData.Length % 3 != 0)
        {
            return new List<Vector3>();
        }
            
        var vertexList = new List<Vector3>();
        for (int i = 0; i < vertexData.Length; i += 3)
        {
            vertexList.Add(new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]));
        }

        return vertexList;
    }
}