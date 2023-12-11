using OpenTK.Mathematics;

namespace Kalasrapier.Engine.Rendering.Services;

public class CollisionManager
{
    public static bool CheckCollision(Actor actor1, Actor actor2)
    {
        return (actor1.Collider, actor2.Collider) switch
        {
            // Sphere Collisions
            (SphereCollider collider1, SphereCollider collider2) => CheckCollisionSphere(collider1, collider2),
            (SphereCollider collider1, BoxCollider collider2) => CheckCollisionSphereAabb(collider1, collider2),
            (SphereCollider collider1, PointCollider collider2) => CheckCollisionSpherePoint(collider1, collider2),
            // Box Collisions
            (BoxCollider collider1, BoxCollider collider2) => CheckCollisionAabb(collider1, collider2),
            (BoxCollider collider1, SphereCollider collider2) => CheckCollisionSphereAabb(collider2, collider1),
            (BoxCollider collider1, PointCollider collider2) => CheckCollisionAabbPoint(collider1, collider2),
            // Points can't collide between each other
            (PointCollider _, PointCollider _) => false,
            (PointCollider collider1, BoxCollider collider2) => CheckCollisionAabbPoint(collider2, collider1),
            (PointCollider collider1, SphereCollider collider2) => CheckCollisionSpherePoint(collider2, collider1),

            _ => false
        };
    }

    private static bool CollisionLess(Vector3 v1, Vector3 v2)
    {
        return v1.X < v2.X && v1.Y < v2.Y && v1.Z < v2.Z;
    }
        
    private static bool CheckCollisionAabb(BoxCollider collider1, BoxCollider collider2)
    {
        var actor1 = collider1.Actor;
        var actor2 = collider2.Actor;
            
        var translation1 = actor1.GetWorldTransform().ExtractTranslation();
        var scale1 = actor1.GetWorldTransform().ExtractScale();
        var translation2 = actor2.GetWorldTransform().ExtractTranslation();
        var scale2 = actor2.GetWorldTransform().ExtractScale();

        var actorBox1 = collider1.BoundingBox.ApplyScaleAndTranslate(scale1, translation1);
        var actorBox2 = collider2.BoundingBox.ApplyScaleAndTranslate(scale2, translation2);

        return CollisionLess(actorBox1.Min, actorBox2.Max) && CollisionLess(actorBox2.Min, actorBox1.Max);
    }

    private static bool CheckCollisionSphere(SphereCollider collider1, SphereCollider collider2)
    {
        var actor1 = collider1.Actor;
        var actor2 = collider2.Actor;
            
        var translation1 = actor1.GetWorldTransform().ExtractTranslation();
        var scale1 = actor1.GetWorldTransform().ExtractScale();
        var translation2 = actor2.GetWorldTransform().ExtractTranslation();
        var scale2 = actor2.GetWorldTransform().ExtractScale();

        var center1 = collider1.Center + translation1;
        var center2 = collider2.Center + translation2;
            
        var distance = Vector3.Distance(center1, center2);
            
        return collider1.Radius * scale1.X + collider2.Radius * scale2.X >= distance;
    }

    private static bool CheckCollisionSphereAabb(SphereCollider collider1, BoxCollider collider2)
    {
        var actor1 = collider1.Actor;
        var actor2 = collider2.Actor;
            
        var translation1 = actor1.GetWorldTransform().ExtractTranslation();
        var scale1 = actor1.GetWorldTransform().ExtractScale();
        var translation2 = actor2.GetWorldTransform().ExtractTranslation();
        var scale2 = actor2.GetWorldTransform().ExtractScale();
            
        var center1 = collider1.Center + translation1;
        var boundingBox = collider2.BoundingBox.ApplyScaleAndTranslate(scale2, translation2);
            
        var distance = Vector3.Distance(center1, boundingBox.ClosestPoint(center1));
            
        // Spheres colliders require an uniform scale, to keep its sphere shape.
        // By default, we use the biggest scale of all axis.
        return distance <= collider1.Radius * Math.Max(Math.Max(scale1.X, scale1.Y), scale1.Z);
    }

    private static bool CheckCollisionSpherePoint(SphereCollider collider1, PointCollider collider2)
    {
        var actor1 = collider1.Actor;
        var actor2 = collider2.Actor;
            
        var translation1 = actor1.GetWorldTransform().ExtractTranslation();
        var scale1 = actor1.GetWorldTransform().ExtractScale();
        var translation2 = actor2.GetWorldTransform().ExtractTranslation();
            
        var center1 = collider1.Center + translation1;
        var point = collider2.Point + translation2;
            
        var distance = Vector3.Distance(center1, point);
        return distance <= collider1.Radius * Math.Max(Math.Max(scale1.X, scale1.Y), scale1.Z);
    }

    private static bool CheckCollisionAabbPoint(BoxCollider collider1, PointCollider collider2)
    {
        var actor1 = collider1.Actor;
        var actor2 = collider2.Actor;
            
        var translation1 = actor1.GetWorldTransform().ExtractTranslation();
        var scale1 = actor1.GetWorldTransform().ExtractScale();
        var translation2 = actor2.GetWorldTransform().ExtractTranslation();
            
        var boundingBox = collider1.BoundingBox.ApplyScaleAndTranslate(scale1, translation1);
        var point = collider2.Point + translation2;
            
        return CollisionLess(boundingBox.Min, point) && CollisionLess(point, boundingBox.Max);
    }
}