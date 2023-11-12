namespace Kalasrapier.Engine.ImportJson
{
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

    static public class VertexInfoMethods
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
        // will keep it like that
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

    public class IndicesPerMaterial
    {
        public uint start { get; set; }
        public uint offset { get; set; }
    }

    public class MeshJson
    {
        // 3 floats
        public float[] vertexdata { get; set; }
        // 2 floats
        public float[]? uv { get; set; }
        // Colors are assume to be 4 floats
        public float[]? colordata { get; set; }
        // 3 floats
        public float[]? normaldata { get; set; }
        // 1 
        public float[]? weightdata { get; set; }
        // uints
        public uint[]? indexdata { get; set; }

        public IndicesPerMaterial[]? index_slots { get; set; }

        public MaterialJson[]? materials { get; set; }


        public MeshJson()
        {
            vertexdata = new float[0];
        }

        public VertexInfo GetInfo()
        {
            var flags = VertexInfo.VERTICES;
            if (colordata is not null)
                flags |= VertexInfo.COLORS;
            if (uv is not null)
                flags |= VertexInfo.UV;
            if (normaldata is not null)
                flags |= VertexInfo.NORMALS;
            if (weightdata is not null)
                flags |= VertexInfo.WEIGHTS;

            return flags;
        }

        // TODO: validate size of each component
        public bool Validate()
        {
            
            return true;
        }

        public void GetVertexArray(out float[] vertexData)
        {
            var info = GetInfo();

            // We can't directly use the vertex length when we work with UVs. Due, that each UV is linked to a vertex
            // inside a triangle primitive and those primitive could have those vertices 0 or more times shared with others.
            // Also, one would be encoured to think that the UV points would match the index points. But it is false
            // depending on the representation of those index points. We will be using one where the indexes aren't
            // reuse between triangles. So, our UVs will always be double size of our index buffer.
            //
            // The code is implemented to allow the case where the vertices, normals, colors... are associate to the
            // each polygon primitive.

            var getSize = (VertexInfo info) => {
                return uv?.Length / 2 * info.ComponentSize();
            };

            var verticesLength = getSize(VertexInfo.VERTICES) ?? vertexdata.Length;
            // Normals are associate to a vertex 1-1. This could change be to achieve a Flat Shading where each triangle
            // would share the face normal.
            var normalLength = getSize(VertexInfo.NORMALS) ?? normaldata?.Length ?? 0;
            // Colors are associate to a vertex 1-1.
            var colorsLength = getSize(VertexInfo.COLORS) ?? colordata?.Length ?? 0;
            // Weights are associate to a vertex 1-1.
            var weightsLength = getSize(VertexInfo.COLORS) ?? weightdata?.Length ?? 0;

            var uvLength = uv?.Length ?? 0;

            var size = verticesLength + normalLength + colorsLength + weightsLength + uvLength;

            var strideSize = info.StrideSize();
            
            var getOffset = (int currentOffset, VertexInfo component) => {
                return currentOffset + (info.HasFlag(component) ? component.ComponentSize() : 0);
            };

            vertexData = new float[size];
            int vI = 0;
            var vertexOffset = getOffset(0, VertexInfo.VERTICES);
            Console.WriteLine("V Offset" + vertexOffset);
            int cI = 0;
            var colorOffset = getOffset(vertexOffset, VertexInfo.COLORS);
            Console.WriteLine("C Offset" + colorOffset);
            int uvI = 0;
            var uvOffset = getOffset(colorOffset, VertexInfo.UV);
            Console.WriteLine("UV Offset" + uvOffset);
            int nI = 0;
            var normalOffset = getOffset(uvOffset, VertexInfo.NORMALS);
            Console.WriteLine("N Offset" + normalOffset);
            int wI = 0;
            var weightsOffset = getOffset(normalOffset, VertexInfo.WEIGHTS);
            Console.WriteLine("W Offset" + weightsOffset);

            Console.WriteLine("VertexData " + size);
            Console.WriteLine("VertexAmount " + vertexdata.Length);
            Console.WriteLine("Color " + colordata.Length);
            Console.WriteLine("normal " + normaldata.Length);

            var fillValuesWithIndices = (ref float[] vertexData, ref int ix, ref int index, float[] array, VertexInfo component) => {
                // When UV is active the index will be associate to Index the indices.
                if (info.HasFlag(VertexInfo.UV))
                {
                    for (int j = 0; j < component.ComponentSize(); j++)
                    {
                        vertexData[ix++] = array[3 * indexdata![index] + j];
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
                    fillValuesWithIndices(ref vertexData, ref i, ref vI, vertexdata, VertexInfo.VERTICES);
                }
                else if (i % strideSize < colorOffset)
                {
                    fillValuesWithIndices(ref vertexData, ref i, ref cI, colordata!, VertexInfo.COLORS);
                }
                else if (i % strideSize < uvOffset)
                {
                    vertexData[i] = uv![uvI];
                    uvI++;
                }
                else if (i % strideSize < normalOffset)
                {
                    fillValuesWithIndices(ref vertexData, ref i, ref nI, normaldata!, VertexInfo.NORMALS);
                }
                else if (i % strideSize < weightsOffset)
                {
                    fillValuesWithIndices(ref vertexData, ref i, ref wI, weightdata!, VertexInfo.WEIGHTS);
                }
            }
        }

        // Drawing by index comes with perfomance penalty. Unless our target machine is memory limited
        // or we need the use of several index buffers to render differents parts of our mesh. The index buffer will
        // only produce a penalty due the simple fact is require a prefetch of those vertices. Where the simple method
        // DrawArrays benefits from data location.
        public void GetIndexArray(out uint[] indexArray)
        {
            if (indexdata is null)
            {
                throw new Exception("Mesh didn't come with indices");
            }

            indexArray = new uint[indexdata!.Length];
            indexdata!.CopyTo(indexArray, 0);
        }
    }
}
