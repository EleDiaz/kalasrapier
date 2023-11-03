using System.Text.Json;

namespace Kalasrapier.Engine.ImportJson
{
    // TODO: This is no longer valid, the scene will be represented by several files, where the level info will
    // be into a different file which can reference to other files where the meshes are.
    // 
    public class SceneLoader
    {
        // TODO: review this comment later
        // IMPLEMENTATION: We can get rid dictionary searchs and replace it with a simple array
        //                 But, the perfomance should be fine this way. So far we are not
        //                 Quering this on each Update with a massive group of meshes.
        private Dictionary<string, MeshFormat> _meshes = new();
        private List<Actor> _actors = new();
        private PlayerStart _playerStart = new();
        private readonly JsonSerializerOptions _jsonOptions = new();

        public SceneLoader() {
        }

        public SceneLoader(string sceneFilePath)
        {
            LoadScene(sceneFilePath);
        }

        public void LoadScene(string sceneFilePath)
        {
            if (sceneFilePath is null)
            {
                throw new Exception("File path is null");
            }
            var sceneData = File.ReadAllText(sceneFilePath);

            var scene = JsonSerializer.Deserialize<Scene>(sceneData, _jsonOptions);
            if (scene is null)
            {
                throw new Exception("Scene is null");
            }

            foreach (var mesh in scene.meshes)
            {
                LoadMesh(mesh.file, mesh.id);
            }
        }

        public void LoadMesh(string file, string id)
        {
            var meshData = File.ReadAllText(file);

            var mesh = JsonSerializer.Deserialize<MeshFormat>(meshData, _jsonOptions);
            if (mesh is null)
            {
                throw new Exception(String.Format("Mesh is null file: {0}, id: {1}", file, id));
            }

            _meshes.Add(id, mesh);
        }

        public int AmountOfMeshes()
        {
            return _meshes.Count;
        }

        public VertexInfo GetVertexInfo(string mesh_id)
        {
            return _meshes[mesh_id].GetInfo();
        }

        public void GetVertexArray(string mesh_id, out float[] vertexData)
        {
            if (_meshes is null)
            {
                throw new Exception("Hasn't been loaded any mesh");
            }
            var mesh = _meshes[mesh_id];
            var info = mesh.GetInfo();

            var size = mesh.vertices.Length + mesh.colors?.Length ?? 0 + mesh.uv?.Length ?? 0 + mesh.normals?.Length ?? 0;

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

        public void GetIndexArray(string mesh_id, out uint[] indexArray, out int[] slots)
        {
            if (_meshes is null)
            {
                throw new Exception("Hasn't been loaded any mesh");
            }
            var mesh = _meshes[mesh_id];
            if (mesh.indicesPerMaterial is null)
            {
                throw new Exception("Mesh didn't come with indices");
            }

            // Lets avoid reallocations
            // Another possible way is to cast the float[][] to float[]
            // but i'm not sure about the internal layout of array in c#
            // I'm guessing that the array would have a size attribute somewhere
            var size = 0;
            foreach (var indeces in mesh.indicesPerMaterial)
            {
                size += indeces.Count();
            }

            // Slots basically tells where the material starts
            slots = new int[0];
            indexArray = new uint[size];
            var ix = 0;
            foreach (var indices in mesh.indicesPerMaterial)
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