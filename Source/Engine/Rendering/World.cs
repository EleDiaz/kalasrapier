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
        public Camera Camera { get; private set; }

        private Scene? _scene;
        public ShaderManager ShaderManager { get; }
        private Meshes Meshes { get; set; } = new();
        private Dictionary<string, Texture> Textures { get; set; } = new();

        public World(GameWindow window)
        {
            Window = window;
            ShaderManager = new ShaderManager();
            Camera = new Camera();
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