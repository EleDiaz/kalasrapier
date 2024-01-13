using System.Text.Json.Serialization;
using Kalasrapier.Engine.Rendering.Actors;

namespace Kalasrapier.Engine.Rendering.Components;

// Use for serialization
// Collisions
[JsonDerivedType(typeof(BoxColliderData), typeDiscriminator: "BoxCollider")]
[JsonDerivedType(typeof(SphereColliderData), typeDiscriminator: "SphereCollider")]
[JsonDerivedType(typeof(PointColliderData), typeDiscriminator: "PointCollider")]

[JsonDerivedType(typeof(MaterialRef), typeDiscriminator: "Material")]
[JsonDerivedType(typeof(CameraData), typeDiscriminator: "Camera")]
[JsonDerivedType(typeof(MeshRef), typeDiscriminator: "Mesh")]
[JsonDerivedType(typeof(DirectionalLightData), typeDiscriminator: "DirectionalLight")]
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