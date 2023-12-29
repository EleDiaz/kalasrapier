using System.Text.Json;
using Kalasrapier.Engine.Rendering.Actors;
using Kalasrapier.Engine.Rendering.Services;

namespace Kalasrapier.Engine.ImportJson;

/// <summary>
/// :: SceneJson -> World -> World
/// </summary>
public class SceneLoader : Base
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

        // Mesh Load
        foreach (var mesh in sceneJson.Meshes)
        {
            MeshManager.AddMeshResource(mesh.File, mesh.Id);
        }

        // Actor Load
        foreach (var actor in sceneJson.Actors)
        { 
            var instancedActor = ActorManager.AddActor(new Actor(actor));
            foreach (var componentData in actor.Components)
            {
                instancedActor.AddComponent(componentData.BuildComponent());
            }
        }

        // Texture Load
        foreach (var texture in sceneJson.Textures)
        {
            TextureManager.AddTexture(texture.Id, texture.Path);
        }
    }
}