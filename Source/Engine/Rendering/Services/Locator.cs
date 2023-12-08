
namespace Kalasrapier.Engine.Rendering.Services {
    
    public class Locator {
        public static TextureManager TextureManager { get; private set; }
        public static MeshManager MeshManager { get; private set; }
        public static ActorManager ActorManager { get; private set; }

        /// <summary>
        /// Build the services with their default options.
        /// This could be later be change to add more specialized services.
        /// </summary>
        public static void Defaults()
        {
            TextureManager = new TextureManager();
            MeshManager = new MeshManager();
            ActorManager = new ActorManager();
        }
    }
}
