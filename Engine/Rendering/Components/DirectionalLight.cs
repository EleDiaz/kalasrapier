using System.Text.Json.Serialization;
using Kalasrapier.Engine.Rendering.Actors;
using OpenTK.Mathematics;

namespace Kalasrapier.Engine.Rendering.Components;

[JsonDerivedType(typeof(DirectionalLightData), typeDiscriminator: "DirectionalLight")]
public class DirectionalLightData : ComponentData
{
    public Vector3? Direction { get; set; }
    public Vector3? Color { get; set; }
    public float? Intensity { get; set; }

    public override Component BuildComponent(Actor actor)
    {
        return new DirectionalLight(actor, Direction, Color, Intensity);
    }
}

public class DirectionalLight : Component
{
    public Vector3 Direction { get; set; } = new(0, 0, -1);
    public Vector3 Color { get; set; } = new(1, 1, 1);
    public float Intensity { get; set; } = 1;
    
    public DirectionalLight(Actor actor) : base(actor)
    {
    }
    public DirectionalLight(Actor actor, Vector3? direction = null, Vector3? color = null, float? intensity = null) :
        this(actor)
    {
        if (direction != null)
        {
            Direction = direction.Value;
        }

        if (color != null)
        {
            Color = color.Value;
        }

        if (intensity != null)
        {
            Intensity = intensity.Value;
        }
    }
}