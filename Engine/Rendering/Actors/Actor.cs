using Kalasrapier.Engine.ImportJson;
using Kalasrapier.Engine.Rendering.Components;
using Kalasrapier.Engine.Rendering.Services;
using OpenTK.Mathematics;

namespace Kalasrapier.Engine.Rendering.Actors;

public class Actor
{
    public int Id { get; set; }
    public string Tag { get; set; } = "NO_ACTOR_ID";
    public Matrix4 Transform { get; set; } = Matrix4.Identity;
    public bool Enabled { get; set; } = true;
    private Actor? Parent { get; set; }
    private List<Actor> Children { get; set; } = new();
    
    // This is a transitional variable set when is being destroyed. This allows a path to fully destroy the
    // Actor.
    public bool ToBeDestroyed { get; private set; } = false;

    public Director Director { get; }
    
    private Dictionary<Type, Component> Components { get; set; } = new();

    public Actor(Director director)
    {
        Director = director;
    }

    public void ImportTemplate(ActorData actorData) {
        Tag = actorData.Tag;
        Enabled = actorData.Enabled;

        var scale = new Vector3(actorData.Scale[0], actorData.Scale[1], actorData.Scale[2]);
        var axis = new Vector3(actorData.Orientation.Axis[0], actorData.Orientation.Axis[1],
            actorData.Orientation.Axis[2]);
        var position = new Vector3(actorData.Position[0], actorData.Position[1], actorData.Position[2]);

        Transform = Matrix4.CreateScale(scale) * Matrix4.CreateFromAxisAngle(axis, actorData.Orientation.Angle) *
                    Matrix4.CreateTranslation(position);

        foreach (var componentData in actorData.Components) {
            Console.WriteLine("Component: " + componentData.GetType());
            AddComponent(componentData.BuildComponent(this));
        }
    }

    public void SetParent(Actor parent)
    {
        var actor = parent;
        while (actor != this && actor is not null)
        {
            actor = actor.Parent;
        }

        if (actor is null)
        {
            Parent?.UnLinkChild(this);
            parent.SetChild(this);
            Parent = parent;
        }
        else
        {
            throw new Exception("Cannot set parent because it would create a loop");
        }
    }

    private void SetChild(Actor actor)
    {
        if (Children.Any(child => child == actor))
        {
            throw new Exception("Cannot set child because it already exists");
        }

        Children.Add(actor);
    }

    public void UnLinkChild(Actor actor)
    {
        Children.Remove(actor);
        actor.Parent = null;
    }

    public void InstantiateAsChild(Actor actor)
    {
        actor.SetParent(this);
        Director.ActorManager.AddActor(actor);
    }

    public virtual void Start()
    {
    }

    public virtual void Update(double deltaTime)
    {
    }

    public virtual void OnTriggerEnter(Actor actor)
    {
    }

    public virtual void OnTriggerStay(Actor actor)
    {
    }

    public virtual void OnTriggerExit(Actor actor)
    {
    }

    protected virtual void RenderImGui()
    {
    }

    // TODO: review order of matrix multiplication
    public Matrix4 GetWorldTransform()
    {
        if (Parent is null)
        {
            return Transform;
        }

        return Parent.GetWorldTransform() * Transform;
    }
    
    public T? GetComponent<T>() where T: Component
    {
        // TODO: Review this later
        if (Components.TryGetValue(typeof(T), out var component)) {
            return component as T;
        }
        else {
            return null;
        }
    }
    
    // This implies a lot of problems with the initialization.
    // So the components have to initialized by the user
    // One Way is to use a factory method, it will be a future implementation
    // public T AddComponent<T>() where T: Component, new()
    // {
    //     var component = new T();
    //     component.SetActor(this);
    //     Components.Add(typeof(T).BaseType ?? typeof(T), component);
    //     return component;
    // }
    
    public void AddComponent(Component component)
    {
        component.SetActor(this);
        Components.Add(component.GetType(), component);
    }

    public void Destroy()
    {
        ToBeDestroyed = true;
        Enabled = false;
        foreach (var child in Children)
        {
            child.Destroy();
        }
        Children.Clear();
        
        foreach (var component in Components.Values)
        {
            component.Destroy();
        }
        Components.Clear();
        
        Parent?.UnLinkChild(this);
        
        Director.ActorManager.RemoveActor(this);
    }
}