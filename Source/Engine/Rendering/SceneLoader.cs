using System.Text.Json;
using Kalasrapier.Engine.ImportJson;

namespace Kalasrapier.Engine.Rendering
{
    /// <summary>
    /// :: SceneJson -> World -> World
    /// </summary>
    public class SceneLoader
    {
        public static void LoadScene(string sceneFilePath, World world)
        {
            if (sceneFilePath is null)
            {
                throw new Exception("File path is null");
            }
            var sceneData = File.ReadAllText(sceneFilePath);

            var sceneJson = JsonSerializer.Deserialize<SceneJson>(sceneData);
            if (sceneJson is null)
            {
                throw new Exception("Scene is null");
            }

            foreach (var mesh in sceneJson.meshes)
            {
                world.MeshManager.AddMeshResource(mesh.file, mesh.id);
            }

            var sceneActor = new Actor { Enabled = true, Id = sceneJson.id };
            world.ActorManager.AddActor(sceneActor);
            foreach (var actor in sceneJson.actors)
            {
                world.ActorManager.AddActorFromScene(sceneActor, new Actor(actor));
            }

            foreach (var texture in sceneJson.textures)
            {
                world.TextureManager.AddTexture(texture.id, texture.path);
            }
        }
    }
}