namespace Kalasrapier.Engine.Rendering.Services.MeshManager;

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