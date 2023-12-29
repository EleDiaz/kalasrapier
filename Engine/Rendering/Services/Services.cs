namespace Kalasrapier.Engine.Rendering.Services;

/// <summary>
/// This ain't just a global variable, it is only a set of global variables (only usable from inherit classes)
/// </summary>
public class Base
{
    protected static TextureManager TextureManager { get; private set; } = null!;
    protected static MeshManager MeshManager { get; private set; } = null!;
    protected static ActorManager ActorManager { get; private set; } = null!;
    protected static CollisionManager CollisionManager { get; private set; } = null!;
    protected static World World { get; private set; } = null!;

    /// <summary>
    /// Build the services with their default options.
    /// This could be later be change to add more specialized services.
    /// TODO: This should be somehow private to World class alone.
    ///       We could join the Base and World classes
    /// </summary>
    public static void SetDefaultsBaseWithNewWorld(World world)
    {
        World = world;
        TextureManager = new TextureManager();
        MeshManager = new MeshManager();
        ActorManager = new ActorManager();
        CollisionManager = new CollisionManager();
    }
}

public class BaseSingleton
{
    private static Dictionary<Type, object> SingletonInstances = new();
    
    public static T GetInstance<T>() where T : notnull
    {
        return (T)SingletonInstances[typeof(T)];
    }

    protected static void Subscribe<T>(T instance) where T : notnull
    {
        SingletonInstances.Add(typeof(T), instance);
    }
}

public class Singleton<T> : BaseSingleton where T : notnull
{
    private static T Instance { get; set; } = default!;
    
    public T GetInstance()
    {
        return Instance;
    }

    public void Construct(T value)
    {
        Instance = value;
        Subscribe(Instance);
    }
}