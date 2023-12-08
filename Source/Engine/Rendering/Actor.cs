using Kalasrapier.Engine.ImportJson;
using OpenTK.Mathematics;
using static Kalasrapier.Engine.Rendering.Services.Locator;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace Kalasrapier.Engine.Rendering
{
    public class Actor
    {
        public string? MeshId { get; set; }
        public string? TextureId { get; set; }
        public Matrix4 Transform { get; set; } = Matrix4.Identity;
        public bool Enabled { get; set; } = false;
        public string Id { get; set; } = "NO_ACTOR_ID";
        public CollisionType CollisionType { get; set; } = CollisionType.None;
        private Actor? Parent { get; set; }
        private List<Actor> Children { get; set; }
        

        // A property to be setup once the object has been instanced by the world
        private World? _world;

        public World World
        {
            get => _world!;
            private set => _world = value;
        }

        public string RenderPipeline { get; set; }

        public Actor()
        {
        }

        public Actor(Actor actor)
        {
            TemplateActor(actor);
        }

        public void TemplateActor(Actor actor)
        {
            MeshId = actor.MeshId;
            TextureId = actor.TextureId;
            Transform = actor.Transform;
            Enabled = actor.Enabled;
            Id = actor.Id;
            Parent = actor.Parent;
            Children = actor.Children;
            World = actor.World;
            RenderPipeline = actor.RenderPipeline;
        }

        public Actor(ActorJson actorJson)
        {
            MeshId = actorJson.MeshId;
            TextureId = actorJson.TextureId;
            Id = actorJson.Id;
            Enabled = actorJson.Enabled;

            var scale = new Vector3(actorJson.Scale[0], actorJson.Scale[1], actorJson.Scale[2]);
            var axis = new Vector3(actorJson.Orientation.Axis[0], actorJson.Orientation.Axis[1],
                actorJson.Orientation.Axis[2]);
            var position = new Vector3(actorJson.Position[0], actorJson.Position[1], actorJson.Position[2]);

            Transform = Matrix4.CreateScale(scale) * Matrix4.CreateFromAxisAngle(axis, actorJson.Orientation.Angle) *
                        Matrix4.CreateTranslation(position);
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
            else
            {
                Children.Add(actor);
            }
        }

        public void UnLinkChild(Actor actor)
        {
            Children.Remove(actor);
            actor.Parent = null;
        }

        public void InstantiateAsChild(Actor actor)
        {
            actor.SetParent(this);
            ActorManager.AddActor(actor);
        }

        public void Instantiate(Actor actor)
        {
            ActorManager.AddActor(actor);
        }

        public virtual void Start()
        {
        }

        public virtual void Update(double deltaTime)
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
            else
            {
                return Parent.GetWorldTransform() * Transform;
            }
        }
        
        public void GenerateCollisionData(CollisionType collisionType,Mesh mesh ){
            CollisionData=new List<Vector3>();
            List<Vector3> vertices= Utils.GenerateVector3List(mesh.vertexData);
            Vector3 ?Min=null;
            Vector3 ?Max=null;

            Collision.CollisionMinMax(vertices,out Min, out Max);
            if(Min is not null && Max is not null){
                CollisionData.Add(Min.Value);
                CollisionData.Add(Max.Value);
            }
        }

        public List<Vector3>? GetCollisionBox(){
            if(CollisionData.Count<2)
                return null;
            List<Vector3> result=CollisionData.GetRange(0,2); // Design decision, return only the required vertices for a Box.
            return result;
        }

    }
}