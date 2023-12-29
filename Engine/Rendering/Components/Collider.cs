using System.Text.Json.Serialization;
using Kalasrapier.Engine.Rendering.Actors;
using OpenTK.Mathematics;

namespace Kalasrapier.Engine.Rendering.Components;

public class Collider : Component;

public record AabbBoundingBox(Vector3 Min = new(), Vector3 Max = new())
{
    public AabbBoundingBox ApplyScaleAndTranslate(Vector3 scale, Vector3 translate)
    {
        return new AabbBoundingBox(Min * scale + translate, Max * scale + translate);
    }

    public Vector3 ClosestPoint(Vector3 point)
    {
        var x = Math.Max(Min.X, Math.Min(point.X, Max.X));
        var y = Math.Max(Min.Y, Math.Min(point.Y, Max.Y));
        var z = Math.Max(Min.Z, Math.Min(point.Z, Max.Z));
        return new Vector3(x, y, z);
    }
}

[JsonDerivedType(typeof(BoxColliderData), typeDiscriminator: "BoxCollider")]
public class BoxColliderData : ComponentData
{
    public bool AutoGenerate = true;
    public Vector3? Scale { get; set; }
    public Vector3? Translate { get; set; }
    
    public override Component BuildComponent()
    {
        if (AutoGenerate)
        {
            var boxCollider = new BoxCollider();
            boxCollider.Recalculate();
        }
        else
        {
            BoundingBox = new AabbBoundingBox(initData.Translate!.Value, initData.Scale!.Value);
        }
    }
}

public class BoxCollider : Collider
{
    public AabbBoundingBox BoundingBox { get; private set; } = new();

    /// <summary>
    /// This should be needed when we apply rotations to the Actor. Due to a limitation of aabb implementation.
    /// </summary>
    public void Recalculate()
    {
        var mesh = Actor!.GetComponent<Mesh>();
        if (mesh is null)
        {
            return; // TODO:
        }

        var vertexList = mesh.GetVertexList();
        BoundingBox = CollisionMinMax(vertexList);
    }

    private static AabbBoundingBox CollisionMinMax(List<Vector3> vertexList)
    {
        var min = Vector3.Zero;
        var max = Vector3.Zero;

        // Get the min and max values of the mesh to get the bounding box that cover all the vertices.
        // This works for any kind of shape.
        foreach (var v in vertexList)
        {
            if (v.X < min.X)
                min.X = v.X;
            if (v.Y < min.Y)
                min.Y = v.Y;
            if (v.Z < min.Z)
                min.Z = v.Z;

            if (v.X > max.X)
                max.X = v.X;
            if (v.Y > max.Y)
                max.Y = v.Y;
            if (v.Z > max.Z)
                max.Z = v.Z;
        }

        return new AabbBoundingBox(min, max);
    }
}

[JsonDerivedType(typeof(SphereColliderData), typeDiscriminator: "SphereCollider")]
public class SphereColliderData : ComponentData
{
    public bool AutoGenerate = true;
    public float? Radius { get; set; }
    public Vector3? Center { get; set; }
}

public class SphereCollider : Collider
{
    public float Radius { get; set; }
    public Vector3 Center { get; set; }

    public SphereCollider(Actor actor) : base(actor)
    {
        Radius = 0;
        Center = Vector3.Zero;
    }

    public SphereCollider(Actor actor, float radius, Vector3 center) : base(actor)
    {
        Radius = radius;
        Center = center;
    }

    public void Recalculate()
    {
        // Generate a default sphere
        var mesh = Actor?.GetComponent<Mesh>();
        if (mesh is null)
        {
            return; // TODO:
        }

        var vertexList = mesh.GetVertexList();
        Center = PointCollider.GetMean(vertexList);
        Radius = GetMaxDistanceFromPoint(Center, vertexList);
    }

    public override void Init(ComponentData componentData)
    {
        base.Init(componentData);
        var initData = componentData as SphereColliderData ??
                       throw new Exception("Invalid component data expected: " + nameof(SphereColliderData));
        if (initData.AutoGenerate)
        {
            Recalculate();
        }
        else
        {
            Radius = initData.Radius!.Value;
            Center = initData.Center!.Value;
        }
    }

    private static float GetMaxDistanceFromPoint(Vector3 center, List<Vector3> vertices)
    {
        return vertices.Aggregate(0f, (current, v) =>
        {
            var newDistance = Vector3.Distance(v, center);
            return newDistance > current ? newDistance : current;
        });
    }
}

[JsonDerivedType(typeof(PointColliderData), typeDiscriminator: "PointCollider")]
public class PointColliderData : ComponentData
{
    public bool AutoGenerate = true;
    public Vector3? Point { get; set; }
}

public class PointCollider : Collider
{
    public Vector3 Point { get; set; }

    public void Recalculate()
    {
        var mesh = Actor?.GetComponent<Mesh>();
        if (mesh is null)
        {
            return; // TODO:
        }

        Point = GetMean(mesh.GetVertexList());
    }

    public PointCollider(Actor actor, Vector3 point) : base(actor)
    {
        Point = point;
    }

    public static Vector3 GetMean(List<Vector3> vertexList)
    {
        return vertexList.Aggregate(Vector3.Zero, (current, v) => current + v) / vertexList.Count;
    }

    public override void Init(ComponentData componentData)
    {
        base.Init(componentData);
        var initData = componentData as PointColliderData ??
                       throw new Exception("Invalid component data expected: " + nameof(PointColliderData));
        if (initData.AutoGenerate)
        {
            Recalculate();
        }
        else
        {
            Point = initData.Point!.Value;
        }
    }
}