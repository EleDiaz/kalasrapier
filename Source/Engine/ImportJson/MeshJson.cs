namespace Kalasrapier.Engine.ImportJson
{
    /// <summary>
    /// This enum defines the accepted format for each component
    /// </summary>
    [Flags]
    public enum VertexInfo {
        VERTICES = 1,
        COLORS = 2,
        UV = 4,
        NORMALS = 8,
        WEIGHTS = 16,
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


    public class MeshJson
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

        public MaterialJson[]? materialSlot { get; set; }


        public MeshJson() {
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

        public void GetVertexArray(out float[] vertexData)
        {
            var info = GetInfo();

            var size = vertices.Length + colors?.Length ?? 0 + uv?.Length ?? 0 + normals?.Length ?? 0 + weights?.Length ?? 0;

            var strideSize = info.StrideSize();

            vertexData = new float[size];
            int vI = 0;
            var vertexOffset = VertexInfo.VERTICES.ComponentSize();
            int cI = 0;
            var colorOffset = vertexOffset + (info.HasFlag(VertexInfo.COLORS) ? VertexInfo.COLORS.ComponentSize() : 0);
            int uvI = 0;
            var uvOffset = colorOffset + (info.HasFlag(VertexInfo.UV) ? VertexInfo.UV.ComponentSize() : 0);
            int nI = 0;
            var normalOffset = uvOffset + (info.HasFlag(VertexInfo.NORMALS) ? VertexInfo.NORMALS.ComponentSize() : 0);
            int wI = 0;
            var weightsOffset = normalOffset + (info.HasFlag(VertexInfo.WEIGHTS) ? VertexInfo.NORMALS.ComponentSize() : 0);

            for (int i = 0; i < vertexData.Length; i++)
            {
                if (i % strideSize < vertexOffset)
                {
                    vertexData[i] = vertices[vI];
                    vI++;
                }
                else if (i % strideSize < colorOffset)
                {
                    vertexData[i] = colors![cI];
                    cI++;
                }
                else if (i % strideSize < uvOffset)
                {
                    vertexData[i] = uv![uvI];
                    uvI++;
                }
                else if (i % strideSize < normalOffset)
                {
                    vertexData[i] = normals![nI];
                    nI++;
                }
                else if (i % strideSize < weightsOffset)
                {
                    vertexData[i] = normals![wI];
                    wI++;
                }
            }
        }

        public void GetIndexArray(out uint[] indexArray, out int[] slots)
        {
            if (indicesPerMaterial is null)
            {
                throw new Exception("Mesh didn't come with indices");
            }

            // Lets avoid reallocations
            // Another possible way is to cast the float[][] to float[]
            // but i'm not sure about the internal layout of array in c#
            // I'm guessing that the array would have a size attribute somewhere
            var size = 0;
            foreach (var indeces in indicesPerMaterial)
            {
                size += indeces.Count();
            }

            // Slots basically tells where the material starts
            slots = new int[0];
            indexArray = new uint[size];
            var ix = 0;
            foreach (var indices in indicesPerMaterial)
            {
                slots.Append(ix);
                foreach (var index in indices)
                {
                    indexArray[ix] = index;
                    ix++;
                }
            }
        }
    }
}
