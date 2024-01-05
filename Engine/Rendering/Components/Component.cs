using Kalasrapier.Engine.Rendering.Actors;

namespace Kalasrapier.Engine.Rendering.Components;

// Use for serialization
public abstract class ComponentData
{
    public abstract Component BuildComponent(Actor actor);
}

public abstract class Component
{
    public Actor Actor { get; private set; }

    public Component(Actor actor)
    {
        Actor = actor;
    }

    public virtual void Destroy()
    {
    }

    public void SetActor(Actor actor)
    {
        Actor = actor;
    }
}