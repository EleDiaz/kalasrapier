using Kalasrapier.Engine.ImportJson;
using OpenTK.Mathematics;
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
        
        private Actor? Parent { get; set; }
        private List<Actor> Children { get; set; }
        
        // A property to be setup once the object has been instanced by the world
        private World? _world;
        public World World { get => _world!; set => _world = value; }
        public ulong RenderPipeline { get; set; }

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
            MeshId = actorJson.mesh_id;
            TextureId = actorJson.texture_id;
            Id = actorJson.id;
            Enabled = actorJson.enabled;

            var scale = new Vector3(actorJson.scale[0], actorJson.scale[1], actorJson.scale[2]);
            var axis = new Vector3(actorJson.orientation.axis[0], actorJson.orientation.axis[1], actorJson.orientation.axis[2]);
            var position = new Vector3(actorJson.position[0], actorJson.position[1], actorJson.position[2]);

            Transform = Matrix4.CreateScale(scale) * Matrix4.CreateFromAxisAngle(axis, actorJson.orientation.angle) * Matrix4.CreateTranslation(position);
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
            World.ActorManager.AddActor(actor);
        }

        public void Instantiate(Actor actor)
        {
            World.ActorManager.AddActor(actor);
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
    }
}
