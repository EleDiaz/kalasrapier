using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Desktop;

namespace Kalasrapier.Engine.Rendering
{

    public class World
    {
        public GameWindow Window { get; private set; }
        public Camera Camera { get; private set; } = new();
        public ActorManager ActorManager { get; set; } = new();
        public MeshManager MeshManager { get; set; } = new();
        public TextureManager TextureManager { get; set; } = new();

        private List<RenderPipeline> _renderPipelines = new();

        public World(GameWindow window)
        {
            Window = window;
        }

        public void LoadScene(string path)
        {
            SceneLoader.LoadScene(path, this);
            // Upgrade the Camera
            Camera = ActorManager.ExtendActorBehavior<Camera>(Camera.TAG);
        }
        
        public ulong AddRenderPipeline(RenderPipeline renderPipeline)
        {
            _renderPipelines.Add(renderPipeline);
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
                renderPipeline
                    .Render(
                        ActorManager.GetActors()
                            .Where(actor =>
                                actor.Enabled && (renderPipeline.Id & actor.RenderPipeline) == renderPipeline.Id),
                        Camera,
                        MeshManager, TextureManager);
            }
        }
    }
}