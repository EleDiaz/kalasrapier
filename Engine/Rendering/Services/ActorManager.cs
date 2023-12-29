using Kalasrapier.Engine.ImportJson;
using Kalasrapier.Engine.Rendering.Actors;

namespace Kalasrapier.Engine.Rendering.Services;

/// 
public class ActorManager
{
    // Raw data of actors, those get indexed by a tag. These are suppose to be templates,
    // as you could get those to build a swarm of actors with the same tag but different Ids.
    private Dictionary<string, ActorData> ActorsData { get; set; } = new();

    // Hashmap of actors, indexed by generated id, wont be two actors with the same id
    private Dictionary<int, Actor> Actors { get; set; } = new();
    
    // ID counter
    private int NextId { get; set; } = 1;

    public T AddActor<T>(T actor)
        where T : Actor
    {
        actor.Id = NextId;
        Actors.Add(NextId++, actor);
        return actor;
    }
    
    public ActorData GetTemplateActorData(string tag)
    {
        return ActorsData[tag];
    }

    /// <summary>
    /// Extend an actor from the scene, replacing it with a subclass of Actor given in parameter.
    /// It will copy the Actor base class properties to the new one.
    /// </summary>
    /// <param name="actor"></param>
    /// <param name="id"></param> actor id
    /// <typeparam name="T"></typeparam>
    public void OverwriteActor<T>(T actor, int id)
        where T : Actor, new()
    {
        actor.TemplateActor(Actors[id]);
        Actors[id] = actor;
    }

    public IEnumerable<Actor> FindActorsByTag(string tag)
    {
        return Actors.Values.Where(actor => actor.Tag == tag);
    }

    public Actor FindActor(int actorId)
    {
        return Actors.Values.First(actor => actor.Id == actorId);
    }
    
    public IEnumerable<Actor> GetActors()
    {
        return Actors.Values;
    }
}
