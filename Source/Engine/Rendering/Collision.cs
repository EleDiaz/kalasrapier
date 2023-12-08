using OpenTK.Mathematics;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace Kalasrapier.Engine.Rendering
{
    [Flags]
    public enum CollisionType : byte
    {
        None,
        Box,
        Sphere,
    }
    
    public record AabbBoundingBox(Vector3 Min, Vector3 Max);
    

    public class Collision
    {
        public static bool CollisionLess(Vector3 v1, Vector3 v2)
        {
            return v1.X < v2.X && v1.Y < v2.Y && v1.Z < v2.Z;
        }

        static public void CollisionMinMax(List<Vector3> list, out Vector3? Min, out Vector3? Max)
        {
            Min = null;
            Max = null;
            foreach (Vector3 v in list)
            {
                if (CollisionLess(v, Min))
                {
                    Min = v;
                }

                if (CollisionLess(Max, v))
                {
                    Max = v;
                }
            }
        }

        public static bool CheckCollisionAABB(Actor actor1, Actor actor2)
        {
            bool result = false;

            Vector3 v1 = actor1.GetWorldTransform().ExtractTranslation();
            Vector3 scale1 = actor1.GetWorldTransform().ExtractScale();
            Vector3 v2 = actor2.GetWorldTransform().ExtractTranslation();
            Vector3 scale2 = actor2.GetWorldTransform().ExtractScale();
            
            List<Vector3>? actorBox1 = actor1.GetCollisionBox();
            List<Vector3>? actorBox2 = actor2.GetCollisionBox();
            if (actorBox1 is not null && actorBox2 is not null)
            {
                List<Vector3> box1 = new List<Vector3>();
                List<Vector3> box2 = new List<Vector3>();

                // Translate each box
                // TODO: scale !!!!!d

                foreach (Vector3 v in actorBox1)
                    box1.Add(v + v1);

                foreach (Vector3 v in actorBox2)
                    box2.Add(v + v2);

                // Check AABB Collision

                if (CollisionLess(box1[0], box2[1]) && CollisionLess(box2[0], box1[1]))
                    result = true;
            }

            return result;
        }
    }
}