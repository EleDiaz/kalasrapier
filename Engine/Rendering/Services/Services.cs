namespace Kalasrapier.Engine.Rendering.Services;

public class Locator
{
    public static TextureManager TextureManager { get; private set; } = null!;
    public static MeshManager MeshManager { get; private set; } = null!;
    public static ActorManager ActorManager { get; private set; } = null!;
    public static World World { get; private set; } = null!;

    /// <summary>
    /// Build the services with their default options.
    /// This could be later be change to add more specialized services.
    /// </summary>
    public static void Defaults(World world)
    {
        World = world;
        TextureManager = new TextureManager();
        MeshManager = new MeshManager();
        ActorManager = new ActorManager();
    }
}