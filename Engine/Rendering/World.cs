using Kalasrapier.Engine.ImportJson;
using Kalasrapier.Engine.Rendering.Services;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Desktop;

namespace Kalasrapier.Engine.Rendering;

public class World
{
    public GameWindow Window { get; private set; }

    private List<RenderPipeline> _renderPipelines = [];

    public World(GameWindow window)
    {
        Window = window;
        // Initialize the services.
        Locator.Defaults(this);
    }

    public void LoadScene(string path)
    {
        SceneLoader.LoadScene(path);
        // Upgrade the Camera
        var camera = Locator.ActorManager.ExtendActorBehavior<Camera>(Camera.Tag);
        Locator.ActorManager.SetMainCamera(camera);
    }

    public ulong AddRenderPipeline(RenderPipeline renderPipeline)
    {
        _renderPipelines.Add(renderPipeline);
        // TODO: Setup
        return 0;
    }

    public void Start()
    {
        foreach (var actor in Locator.ActorManager.GetActors().Where(actor => actor.Enabled))
        {
            actor.Start();
        }
    }

    public void Update(double deltaTime)
    {
        foreach (var actor in Locator.ActorManager.GetActors().Where(actor => actor.Enabled))
        {
            actor.Update(deltaTime);
        }
    }

    public void Render()
    {
        foreach (var renderPipeline in _renderPipelines)
        {
            // Future implementations could use some frustum clipping to filter even more the actors
            //
            renderPipeline
                .Render(
                    Locator.ActorManager.GetActors()
                        .Where(actor =>
                            actor.Enabled && renderPipeline.Tag == actor.RenderPipeline));
        }
    }
}