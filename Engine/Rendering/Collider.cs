using Kalasrapier.Engine.Rendering.Services;
using OpenTK.Mathematics;

namespace Kalasrapier.Engine.Rendering;

[Serializable]
public enum ColliderType
{
    None,
    Box,
    Sphere,
    Point
}

public class Collider
{
    public Actor Actor { get; }

    public Collider(Actor actor)
    {
        Actor = actor;
    }
}

public record AabbBoundingBox(Vector3 Min, Vector3 Max)
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

public class BoxCollider : Collider
{
    public AabbBoundingBox BoundingBox { get; private set; }

    public BoxCollider(Actor actor) : base(actor)
    {
        // generate aabb
        var vertexList = Locator.MeshManager.GetVertexList(actor.MeshId);
        BoundingBox = CollisionMinMax(vertexList);
    }

    /// <summary>
    /// This should be needed when we apply rotations to the Actor. Due to a limitation of aabb implementation.
    /// </summary>
    public void Recalculate()
    {
        var vertexList = Locator.MeshManager.GetVertexList(Actor.MeshId);
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

public class SphereCollider : Collider
{
    public float Radius { get; set; }
    public Vector3 Center { get; set; }

    public SphereCollider(Actor actor) : base(actor)
    {
        // Generate a default sphere
        var vertexList = Locator.MeshManager.GetVertexList(actor.MeshId);
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

    public SphereCollider(Actor actor, float radius, Vector3 center) : base(actor)
    {
        Radius = radius;
        Center = center;
    }
}

public class PointCollider : Collider
{
    public Vector3 Point { get; set; }

    /// <summary>
    /// Get the mean of the vertices of the mesh. And use it as the point of the collider.
    /// </summary>
    /// <param name="actor"></param>
    public PointCollider(Actor actor) : base(actor)
    {
        Point = GetMean(Locator.MeshManager.GetVertexList(actor.MeshId));
    }

    public PointCollider(Actor actor, Vector3 point) : base(actor)
    {
        Point = point;
    }

    public static Vector3 GetMean(List<Vector3> vertexList)
    {
        return vertexList.Aggregate(Vector3.Zero, (current, v) => current + v) / vertexList.Count;
    }
}