using System.Text.Json;
using Kalasrapier.Engine.ImportJson;

namespace Kalasrapier.Engine.Rendering
{
    public class Scene
    {
        // Json Stuff
        private SceneJson? sceneJson;
        private Dictionary<string, MeshJson> meshesJson;

        // Practical Content
        private Meshes _meshes = new();
        public Meshes Meshes { get => _meshes; }
        private Dictionary<string, Actor> _actors = new();
        public Dictionary<string, Actor> Actors { get => _actors; }

        private PlayerStartJson _playerStart = new(); // TODO:

        // Others
        private readonly JsonSerializerOptions _jsonOptions = new();


        public Scene()
        {
            meshesJson = new();
        }

        public Scene(string sceneFilePath)
        {
            meshesJson = new();
            LoadScene(sceneFilePath);
        }

        public void LoadScene(string sceneFilePath)
        {
            if (sceneFilePath is null)
            {
                throw new Exception("File path is null");
            }
            var sceneData = File.ReadAllText(sceneFilePath);

            sceneJson = JsonSerializer.Deserialize<SceneJson>(sceneData, _jsonOptions);
            if (sceneJson is null)
            {
                throw new Exception("Scene is null");
            }

            _playerStart = sceneJson.playerStart;

            foreach (var mesh in sceneJson.meshes)
            {
                LoadMesh(mesh.file, mesh.id);
            }

        }

        public void LoadMesh(string file, string id)
        {
            var meshData = File.ReadAllText(file);

            var mesh = JsonSerializer.Deserialize<MeshJson>(meshData, _jsonOptions);
            if (mesh is null)
            {
                throw new Exception(String.Format("Mesh is null file: {0}, id: {1}", file, id));
            }

            meshesJson.Add(id, mesh);
        }

        // Para el tema de los materiales aquí quizás haya que tocar el Index. Y alguna info addicional de 
        // los materiales
        public void InitScene()
        {
            if (sceneJson is null)
            {
                return;
            }

            foreach (var actor in sceneJson.actors)
            {
                // Load only meshes in into the GPU
                if (actor.enabled)
                {
                    if (!Meshes.MeshesInfo.ContainsKey(actor.sm))
                    {
                        var meshFormat = meshesJson[actor.sm];
                        Meshes.LoadMeshFormat(actor.sm, meshFormat);
                    }
                    Actors.Add(actor.id, new Actor(actor));
                }
            }
        }
    }
}