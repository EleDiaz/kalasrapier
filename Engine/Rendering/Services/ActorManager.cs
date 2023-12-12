namespace Kalasrapier.Engine.Rendering.Services;

/// 
public class ActorManager : Locator
{
    private Dictionary<string, Actor> Actors { get; set; } = new();
    // private List<Actor> Actors { get; set } = new();

    public void AddActor(Actor actor)
    {
        // Actors.Add(actor);
        Actors.Add(actor.Id, actor);
    }

    public void AddActorFromScene(Actor scene, Actor actor)
    {
        actor.SetParent(scene);
        Actors.Add(actor.Id, actor);
    }

    public IEnumerable<Actor> GetActors()
    {
        return Actors.Values;
    }

    /// <summary>
    /// Extend an actor from the scene, replacing it with a subclass of Actor given in parameter.
    /// It will copy the Actor base class properties to the new one.
    /// </summary>
    /// <param name="id"></param> actor id
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T ExtendActorBehavior<T>(string id)
        where T : Actor, new()
    {
        var newActor = new T();
        newActor.TemplateActor(Actors[id]);
        Actors[id] = newActor;
        return newActor;
    }

    public Actor GetActor(string tag)
    {
        return Actors[tag];
    }

    public void SetMainCamera(Camera camera)
    {
        Actors[camera.Id] = camera;
    }

    public Camera GetMainCamera()
    {
        return (Camera)Actors["CAMERA"];
    }
}