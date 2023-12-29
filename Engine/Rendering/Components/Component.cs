
using Kalasrapier.Engine.Rendering.Actors;

namespace Kalasrapier.Engine.Rendering.Components;

// Use for serialization
public abstract class ComponentData
{
    public abstract Component BuildComponent();
}

public abstract class Component
{
    public Actor? Actor { get; set; }
    
    public virtual void SetActor(Actor actor)
    {
        Actor = actor;
    }

    public virtual void Destroy()
    {
        Actor = null;
    }
}