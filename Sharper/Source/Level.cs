using System.Text.Json;

namespace Kalasrapier
{
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

        public void GetVertexArray(int index, out float[] vertexData)
        {
            if (_levelMeshes is null)
            {
                throw new Exception("Hasn't been loaded any mesh");
            }
            var mesh = _levelMeshes[index];
            var info = mesh.GetInfo();

            var size = mesh.vertices.Length + mesh.colors?.Length ?? 0 + mesh.uv?.Length ?? 0 + mesh.normals?.Length ?? 0;

            var strideSize = mesh.StrideSize(info);

            vertexData = new float[size];
            int vI = 0;
            var vertexOffset = mesh.ComponentSize(MeshInfo.VERTICES);
            int cI = 0;
            var colorOffset = vertexOffset + (info.HasFlag(MeshInfo.COLORS) ? mesh.ComponentSize(MeshInfo.COLORS) : 0);
            int uvI = 0;
            var uvOffset = colorOffset + (info.HasFlag(MeshInfo.UV) ? mesh.ComponentSize(MeshInfo.UV) : 0);
            int nI = 0;
            var normalOffset = uvOffset + (info.HasFlag(MeshInfo.NORMALS) ? mesh.ComponentSize(MeshInfo.NORMALS) : 0);

            for (int i = 0; i < vertexData.Length; i++)
            {
                if (i % strideSize < vertexOffset)
                {
                    vertexData[i] = mesh.vertices[vI];
                    vI++;
                }
                else if (i % strideSize < colorOffset) {
                    vertexData[i] = mesh!.colors![cI];
                    cI++;
                }
                else if (i % strideSize < uvOffset) {
                    vertexData[i] = mesh!.uv![uvI];
                    uvI++;
                }
                else if (i % strideSize < normalOffset) {
                    vertexData[i] = mesh!.normals![nI];
                    nI++;
                }
            }
        }

        public void GetIndexArray(int index, out uint[] indexArray) {
            if (_levelMeshes is null)
            {
                throw new Exception("Hasn't been loaded any mesh");
            }
            var mesh = _levelMeshes[index];
            if (!mesh.GetInfo().HasFlag(MeshInfo.INDICES)) {
                throw new Exception("Mesh didn't come with indices");
            }
            indexArray = mesh.indices!;
        }
    }
}