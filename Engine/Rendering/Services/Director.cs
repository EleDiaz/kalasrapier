using System.Text.Json;
using Kalasrapier.Engine.ImportJson;
using Kalasrapier.Engine.Rendering.Actors;
using Kalasrapier.Engine.Rendering.Components;
using OpenTK.Windowing.Desktop;

namespace Kalasrapier.Engine.Rendering.Services;

public class Director
{
    public GameWindow Window { get; private set; }

    private List<RenderPipeline> _renderPipelines = new();
    public ActorManager ActorManager { get; } = new();
    public MeshManager.MeshManager MeshManager { get; } = new();
    public MaterialManager MaterialManager { get; } = new();
    public CollisionManager CollisionManager { get; }
    public CameraManager Cameras { get; } = new();

    public Director(GameWindow window)
    {
        Window = window;
        CollisionManager = new CollisionManager(ActorManager);
    }

    public void LoadScene(string sceneFilePath)
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

        // Texture Load
        foreach (var texture in sceneJson.Textures)
        {
            MaterialManager.AddTexture(texture.Id, texture.Path);
        }

        foreach (var materialData in sceneJson.Materials)
        {
            var material = new MaterialData {Slots = materialData.Value.ToList() }; 
            Console.WriteLine("Material: " + materialData.Key);
            MaterialManager.AddMaterial(materialData.Key, material);
        }

        // Mesh Load
        foreach (var mesh in sceneJson.Meshes)
        {
            MeshManager.AddMeshResource(mesh.File, mesh.Id);
        }

        // Actor Templates
        foreach (var actorData in sceneJson.Templates)
        {
            ActorManager.LoadActorData(actorData);
        }

        // Actor Load
        foreach (var actorData in sceneJson.Actors)
        {
            var actor = new Actor(this);
            actor.ImportTemplate(actorData);
            ActorManager.AddActor(actor);
        }
    }

    public void AddRenderPipeline(RenderPipeline renderPipeline)
    {
        _renderPipelines.Add(renderPipeline);
    }

    public void Start()
    {
        // Pre-init all actors that belong to a render pipeline even those disabled
        foreach (var renderPipeline in _renderPipelines)
        {
            renderPipeline.Setup(ActorManager.GetActors(), this);
        }

        foreach (var actor in ActorManager.GetActors().Where(actor => actor.Enabled))
        {
            actor.Start();
        }
    }

    public void Update(double deltaTime)
    {
        foreach (var actor in ActorManager.GetActors().Where(actor => actor.Enabled))
        {
            actor.Update(deltaTime);
        }
    }

    public void Render()
    {
        foreach (var renderPipeline in _renderPipelines)
        {
            // TODO: Future implementations could use some frustum clipping to filter even more the actors
            //       With some aabb to fast detection; if the actor is inside the frustum
            renderPipeline
                .Render(
                    ActorManager.GetActors()
                        .Where(actor =>
                            actor.Enabled && renderPipeline.BelongsToPipeline(actor)), this);
        }
    }
}