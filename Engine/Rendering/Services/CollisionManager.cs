using Kalasrapier.Engine.Rendering.Actors;
using Kalasrapier.Engine.Rendering.Components;
using OpenTK.Mathematics;

namespace Kalasrapier.Engine.Rendering.Services;

public class CollisionManager
{
    private readonly ActorManager _actorManager;
    // Actual collisions going on.
    private HashSet<(int, int)> _collisionsLate = new();
    private HashSet<(int, int)> _collisionsNew = new();
    
    // TODO: Check if the ToBeDestroyed flag is set, and remove the actor from the collisions list.
    
    public CollisionManager(ActorManager actorManager)
    {
        _actorManager = actorManager;
    }

    private void CollisionDetected(Actor actor1, Actor actor2)
    {
        // We sort the ids to avoid duplicates in the hashset, and events skips
        var id1 = actor1.Id;
        var id2 = actor2.Id;
        if (id1 > id2)
        {
            (id1, id2) = (id2, id1);
        }

        if (_collisionsLate.Contains((id1, id2)))
        {
            actor1.OnTriggerStay(actor2);
            actor2.OnTriggerStay(actor1);
            _collisionsLate.Remove((id1, id2));
        }
        if (_collisionsNew.Add((id1, id2)))
        {
            actor1.OnTriggerEnter(actor2);
            actor2.OnTriggerEnter(actor1);
        }
    }

    public void CheckCollisions()
    {
        var actors = _actorManager.GetActors().Where(actor => actor.GetComponent<BoxCollider>() is not null)
            .Select((actor, i) => (actor, i)).ToList();
        if (actors.Count < 2)
            return;

        for (int i = 0; i < actors.Count - 1; i++)
        {
            for (int j = i + 1; j < actors.Count; j++)
            {
                var actor1 = actors[i].actor;
                var actor2 = actors[j].actor;
                
                if (CheckCollision(actor1, actor2))
                {
                    CollisionDetected(actor1, actor2);
                }
            }
        }


        foreach (var (actorId1, actorId2) in _collisionsLate)
        {
            var actor1 = _actorManager.FindActor(actorId1);
            var actor2 = _actorManager.FindActor(actorId2);
            actor1.OnTriggerExit(actor2);
            actor2.OnTriggerExit(actor1);
        }

        (_collisionsLate, _collisionsNew) = (_collisionsNew, _collisionsLate);
        _collisionsNew.Clear();
    }


    private static bool CheckCollision(Actor actor1, Actor actor2)
    {
        var c1 = actor1.GetComponent<BoxCollider>() as Collider;
        var c2 = actor2.GetComponent<BoxCollider>() as Collider;
        if (c1 is null || c2 is null)
        {
            return false;
        }
        return (c1, c2) switch
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

        var translation1 = actor1!.GetWorldTransform().ExtractTranslation();
        var scale1 = actor1.GetWorldTransform().ExtractScale();
        var translation2 = actor2!.GetWorldTransform().ExtractTranslation();
        var scale2 = actor2.GetWorldTransform().ExtractScale();

        var actorBox1 = collider1.BoundingBox.ApplyScaleAndTranslate(scale1, translation1);
        var actorBox2 = collider2.BoundingBox.ApplyScaleAndTranslate(scale2, translation2);

        return CollisionLess(actorBox1.Min, actorBox2.Max) && CollisionLess(actorBox2.Min, actorBox1.Max);
    }

    private static bool CheckCollisionSphere(SphereCollider collider1, SphereCollider collider2)
    {
        var actor1 = collider1.Actor;
        var actor2 = collider2.Actor;

        var translation1 = actor1!.GetWorldTransform().ExtractTranslation();
        var scale1 = actor1.GetWorldTransform().ExtractScale();
        var translation2 = actor2!.GetWorldTransform().ExtractTranslation();
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

        var translation1 = actor1!.GetWorldTransform().ExtractTranslation();
        var scale1 = actor1.GetWorldTransform().ExtractScale();
        var translation2 = actor2!.GetWorldTransform().ExtractTranslation();
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

        var translation1 = actor1!.GetWorldTransform().ExtractTranslation();
        var scale1 = actor1.GetWorldTransform().ExtractScale();
        var translation2 = actor2!.GetWorldTransform().ExtractTranslation();

        var center1 = collider1.Center + translation1;
        var point = collider2.Point + translation2;

        var distance = Vector3.Distance(center1, point);
        return distance <= collider1.Radius * Math.Max(Math.Max(scale1.X, scale1.Y), scale1.Z);
    }

    private static bool CheckCollisionAabbPoint(BoxCollider collider1, PointCollider collider2)
    {
        var actor1 = collider1.Actor;
        var actor2 = collider2.Actor;

        var translation1 = actor1!.GetWorldTransform().ExtractTranslation();
        var scale1 = actor1.GetWorldTransform().ExtractScale();
        var translation2 = actor2!.GetWorldTransform().ExtractTranslation();

        var boundingBox = collider1.BoundingBox.ApplyScaleAndTranslate(scale1, translation1);
        var point = collider2.Point + translation2;

        return CollisionLess(boundingBox.Min, point) && CollisionLess(point, boundingBox.Max);
    }
}
