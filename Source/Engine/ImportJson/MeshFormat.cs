namespace Kalasrapier.Engine.ImportJson
{
    /// <summary>
    /// This enum defines the accepted format for each component
    /// </summary>
    [Flags]
    public enum VertexInfo {
        VERTICES = 0,
        COLORS = 1,
        UV = 2,
        NORMALS = 4,
        WEIGHTS = 8,
    }

    static public class VertexInfoMethods {
        public static int StrideSize(this VertexInfo info) {
            return ComponentSize(VertexInfo.VERTICES) 
                + (info.HasFlag(VertexInfo.COLORS)? ComponentSize(VertexInfo.COLORS): 0)
                + (info.HasFlag(VertexInfo.UV)? ComponentSize(VertexInfo.UV): 0)
                + (info.HasFlag(VertexInfo.NORMALS)? ComponentSize(VertexInfo.NORMALS): 0)
                + (info.HasFlag(VertexInfo.WEIGHTS)? ComponentSize(VertexInfo.WEIGHTS): 0);
        }

        // TODO: In our implementation all the subcomponents has a size of 4 bytes (floats and uint) for simplicity i
        // will keep it like that
        public static int StrideOffset(this VertexInfo info) {
            return info.StrideSize() * sizeof(float);
        }

        /// <summary>
        /// Size on bytes of each component.
        /// </summary>
        public static int ComponentSize(this VertexInfo info) {
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
        // 1 
        public float[]? weights { get; set;}
        // uints
        public uint[][]? indicesPerMaterial { get; set;}


        public MeshFormat() {
            vertices = new float[0];
        }

        public VertexInfo GetInfo() {
            var flags = VertexInfo.VERTICES;
            if (colors is not null)
                flags |= VertexInfo.COLORS;
            if (uv is not null)
                flags |= VertexInfo.UV;
            if (normals is not null)
                flags |= VertexInfo.NORMALS;
            if (weights is not null)
                flags |= VertexInfo.WEIGHTS;

            return flags;
        }

        // TODO: validate size of each component
        public bool Validate() {
            return true;
        }
    }
}
