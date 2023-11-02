using System.Text.Json;

namespace Kalasrapier
{
    // TODO: This is no longer valid, the scene will be represented by several files, where the level info will
    // be into a different file which can reference to other files where the meshes are.
    // 
    public class Level
    {
        private string? _meshFilePath;

        private List<MeshFormat>? _levelMeshes = new List<MeshFormat>();
        private readonly JsonSerializerOptions _jsonOptions = new();

        public Level()
        {
        }

        public Level(string pathMeshFile)
        {
            _meshFilePath = pathMeshFile;
        }

        public void LoadLevel()
        {
            if (_meshFilePath is null)
            {
                throw new Exception("File path is null");
            }
            var meshesData = File.ReadAllText(_meshFilePath);
            _levelMeshes = JsonSerializer.Deserialize<List<MeshFormat>>(meshesData, _jsonOptions);
        }

        public int AmountOfMeshes()
        {
            return _levelMeshes!.Count;
        }

        public MeshInfo GetInfo(int index)
        {
            return _levelMeshes![index].GetInfo();
        }

        public void GetVertexArray(int index, out float[] vertexData)
        {
            if (_levelMeshes is null)
            {
                throw new Exception("Hasn't been loaded any mesh");
            }
            var mesh = _levelMeshes[index];
            var info = mesh.GetInfo();

            var size = mesh.vertices.Length + mesh.colors?.Length ?? 0 + mesh.uv?.Length ?? 0 + mesh.normals?.Length ?? 0;

            var strideSize = info.StrideSize();

            vertexData = new float[size];
            int vI = 0;
            var vertexOffset = MeshInfo.VERTICES.ComponentSize();
            int cI = 0;
            var colorOffset = vertexOffset + (info.HasFlag(MeshInfo.COLORS) ? MeshInfo.COLORS.ComponentSize() : 0);
            int uvI = 0;
            var uvOffset = colorOffset + (info.HasFlag(MeshInfo.UV) ? MeshInfo.UV.ComponentSize() : 0);
            int nI = 0;
            var normalOffset = uvOffset + (info.HasFlag(MeshInfo.NORMALS) ? MeshInfo.NORMALS.ComponentSize() : 0);

            for (int i = 0; i < vertexData.Length; i++)
            {
                if (i % strideSize < vertexOffset)
                {
                    vertexData[i] = mesh.vertices[vI];
                    vI++;
                }
                else if (i % strideSize < colorOffset)
                {
                    vertexData[i] = mesh!.colors![cI];
                    cI++;
                }
                else if (i % strideSize < uvOffset)
                {
                    vertexData[i] = mesh!.uv![uvI];
                    uvI++;
                }
                else if (i % strideSize < normalOffset)
                {
                    vertexData[i] = mesh!.normals![nI];
                    nI++;
                }
            }
        }

        public void GetIndexArray(int index, out uint[] indexArray)
        {
            if (_levelMeshes is null)
            {
                throw new Exception("Hasn't been loaded any mesh");
            }
            var mesh = _levelMeshes[index];
            if (!mesh.GetInfo().HasFlag(MeshInfo.INDICES))
            {
                throw new Exception("Mesh didn't come with indices");
            }
            indexArray = mesh.indices!;
        }

        /*
        public void GetFirstMesh(out float[]? vertexData, out int[]? indexData, out int[]? slotData, out RetrievedMaterial[]? matData)
        {
            vertexData = null;
            indexData = null;
            slotData = null;
            matData = null;

            if (_levelMeshes is null)
            {
                throw new Exception("Empty list of meshes");
            }

            var mesh = _levelMeshes.First();
            if (mesh.materials is null)
                throw new Exception("Error material slots for mesh are not assigned");
            matData = new RetrievedMaterial[mesh.materials.Length - 1];
            for (int i = 1; i < mesh.materials.Length; i++)
                matData[i - 1] = mesh.materials[i]; // The first one is a default

            int nvertices = mesh.vertexdata.Length;
            int nweight = mesh.weightdata.Length;
            if ((nvertices / 3) != nweight)
                throw new Exception("Number of vertex weights is different of number of vertices");
            vertexData = new float[nvertices + nweight];
            int nvalues = 4; // 3 components per vers, 1 per weight
            for (int i = 0, j = 0, k = 0; i < mesh.vertexdata.Length; i = i + 3, j = j + 1, k = k + nvalues)
            {
                vertexData[k] = mesh.vertexdata[i];
                vertexData[k + 1] = mesh.vertexdata[i + 1];
                vertexData[k + 2] = mesh.vertexdata[i + 2];
                vertexData[k + 3] = mesh.weightdata[j];
            }
            slotData = new int[mesh.materials.Length - 1];
            int nindex = 0;
            for (int i = 0; i < (mesh.materials.Length - 1); i++)
            {
                slotData[i] = nindex;
                nindex += mesh.indexdata[i].Length;

            }

            indexData = new int[nindex];
            int count = 0;
            for (int i = 0; i < (mesh.materials.Length - 1); i++)
            {
                for (int j = 0; j < mesh.indexdata[i].Length; j++)
                    indexData[count++] = mesh.indexdata[i][j];
            }

        }
        */
    }
}