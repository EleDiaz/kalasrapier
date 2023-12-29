using Kalasrapier.Engine.ImportJson;
using Kalasrapier.Engine.Rendering.Actors;
using Kalasrapier.Engine.Rendering.Services;
using OpenTK.Windowing.Desktop;

namespace Kalasrapier.Engine.Rendering;

public class World : Base
{
    public GameWindow Window { get; private set; }

    private List<RenderPipeline> _renderPipelines = new();

    public World(GameWindow window)
    {
        Window = window;
        // Initialize the services.
        SetDefaultsBaseWithNewWorld(this);
    }

    public void LoadScene(string path)
    {
        SceneLoader.LoadScene(path);
        // Upgrade the Camera
        var template = ActorManager.GetTemplateActorData(Camera.Tag);
        var camera = new Camera(template);
        ActorManager.AddActor(camera);
        
        var camera1 = ActorManager.OverwriteActor<Camera>(Camera.Tag);
        ActorManager.SetMainCamera(camera);
    }

    public ulong AddRenderPipeline(RenderPipeline renderPipeline)
    {
        _renderPipelines.Add(renderPipeline);
        // TODO: Setup
        return 0;
    }

    public void Start()
    {
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
            renderPipeline
                .Render(
                    ActorManager.GetActors()
                        .Where(actor =>
                            actor.Enabled && renderPipeline.BelongsToPipeline(actor)));
        }
    }
}