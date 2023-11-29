using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Desktop;

namespace Kalasrapier.Engine.Rendering
{
    public enum SpecialActors
    {
        Camera,
        PlayerStart,
    }

    public class World
    {
        public GameWindow Window { get; private set; }
        public Camera Camera { get; private set; } = new();

        private Scene? _scene;
        public ActorManager ActorManager { get; set; } = new();
        private MeshManager MeshManager { get; set; } = new();
        private TextureManager TextureManager { get; set; } = new();
        
        private RenderPipeline _renderPipeline;

        public World(GameWindow window)
        {
            Window = window;
            _scene = null;
        }

        public void LoadScene(string path)
        {
            _scene = new Scene(path);

            foreach (var actor in _scene!.Actors.Values.Where(actor => actor.Enabled))
            {
                actor.Start();
            }
        }

        public void Update(float deltaTime)
        {
            foreach (var actor in _scene!.Actors.Values.Where(actor => actor.Enabled))
            {
                actor.Update(deltaTime);
            }
        }

        public void Render()
        {
            foreach (var actor in _scene!.Actors.Values.Where(actor => actor.Enabled))
            {
                if (actor.TextureId is not null)
                {
                    _scene.Textures[actor.TextureId].Use(TextureUnit.Texture0);
                }

                if (actor.MeshId is not null)
                {
                    _shader!.SetMatrix4("model", actor.Transform);
                    _shader!.SetMatrix4("view", Camera.GetViewMatrix());
                    _shader!.SetMatrix4("projection", Camera.GetProjectionMatrix());
                    var mesh = _scene.Meshes.MeshesInfo[actor.MeshId];
                    mesh.SetActiveMesh();
                    mesh.DrawMesh(_shader);
                }
            }
        }
    }
}