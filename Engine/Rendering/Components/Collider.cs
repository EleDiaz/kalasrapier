using System.Text.Json.Serialization;
using Kalasrapier.Engine.Rendering.Actors;
using OpenTK.Mathematics;

namespace Kalasrapier.Engine.Rendering.Components;

public class Collider : Component
{
    public Collider(Actor actor) : base(actor)
    {
    }
}

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

public class BoxColliderData : ComponentData
{
    public bool AutoGenerate = true;
    public float[]? Min { get; set; }
    public float[]? Max { get; set; }
    
    public override Component BuildComponent(Actor actor)
    {
        if (AutoGenerate)
        {
            var boxCollider = new BoxCollider(actor);
            boxCollider.Recalculate();
            return boxCollider;
        }
        else
        {
            return new BoxCollider(actor, new AabbBoundingBox(new Vector3(Min![0], Min[1], Min[2]), new Vector3(Max![0], Max[1], Max[2])));
        }
    }
}

public class BoxCollider : Collider
{
    public AabbBoundingBox BoundingBox { get; private set; }

    public BoxCollider(Actor actor): base(actor)
    {
        BoundingBox = new AabbBoundingBox();
        Recalculate();
    }

    public BoxCollider(Actor actor, AabbBoundingBox aabbBoundingBox) : base(actor)
    {
        BoundingBox = aabbBoundingBox;
    }
    
    // Used to generalize the collider type
    // This could mess up some things a little bit
    // TODO: Review
    public new Type GetType()
    {
        return typeof(Collider);
    }

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

public class SphereColliderData : ComponentData
{
    public bool AutoGenerate = true;
    public float? Radius { get; set; }
    public Vector3? Center { get; set; }
    
    public override Component BuildComponent(Actor actor)
    {
        if (AutoGenerate)
        {
            var sphereCollider = new SphereCollider(actor);
            sphereCollider.Recalculate();
            return sphereCollider;
        }
        else
        {
            return new SphereCollider(actor, Radius!.Value, Center!.Value);
        }
    }
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

    public new Type GetType()
    {
        return typeof(Collider);
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

    private static float GetMaxDistanceFromPoint(Vector3 center, List<Vector3> vertices)
    {
        return vertices.Aggregate(0f, (current, v) =>
        {
            var newDistance = Vector3.Distance(v, center);
            return newDistance > current ? newDistance : current;
        });
    }
}

public class PointColliderData : ComponentData
{
    public bool AutoGenerate = true;
    public Vector3? Point { get; set; }
    
    public override Component BuildComponent(Actor actor)
    {
        if (AutoGenerate)
        {
            var pointCollider = new PointCollider(actor);
            pointCollider.Recalculate();
            return pointCollider;
        }
        else
        {
            return new PointCollider(actor, Point!.Value);
        }
    }
}

public class PointCollider : Collider
{
    public Vector3 Point { get; set; }
    
    public PointCollider(Actor actor) : base(actor)
    {
        Recalculate();
    }
    
    public PointCollider(Actor actor, Vector3 point) : base(actor)
    {
        Point = point;
    }

    public new Type GetType()
    {
        return typeof(Collider);
    }
    
    public void Recalculate()
    {
        var mesh = Actor.GetComponent<Mesh>();
        if (mesh is null)
        {
            return; // TODO:
        }

        Point = GetMean(mesh.GetVertexList());
    }

    public static Vector3 GetMean(List<Vector3> vertexList)
    {
        return vertexList.Aggregate(Vector3.Zero, (current, v) => current + v) / vertexList.Count;
    }
}