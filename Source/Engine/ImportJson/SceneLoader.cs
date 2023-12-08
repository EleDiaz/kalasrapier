using System.Text.Json;
using Kalasrapier.Engine.Rendering;
using static Kalasrapier.Engine.Rendering.Services.Locator;

namespace Kalasrapier.Engine.ImportJson
{
    /// <summary>
    /// :: SceneJson -> World -> World
    /// </summary>
    public class SceneLoader
    {
        public static void LoadScene(string sceneFilePath)
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

            foreach (var mesh in sceneJson.Meshes)
            {
                MeshManager.AddMeshResource(mesh.File, mesh.Id);
            }

            var sceneActor = new Actor { Enabled = true, Id = sceneJson.Id };
            ActorManager.AddActor(sceneActor);
            foreach (var actor in sceneJson.Actors)
            {
                ActorManager.AddActorFromScene(sceneActor, new Actor(actor));
            }

            foreach (var texture in sceneJson.Textures)
            {
                TextureManager.AddTexture(texture.Id, texture.Path);
            }
        }
    }
}