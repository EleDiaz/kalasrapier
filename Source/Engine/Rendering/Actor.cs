using Kalasrapier.Engine.ImportJson;
using OpenTK.Mathematics;

namespace Kalasrapier.Engine.Rendering
{
    public class Actor
    {
        public string? ShaderId { get; set; }
        public string? MeshId { get; set; }
        public string? TextureId { get; set; }
        public Matrix4 Transform { get; set; } = Matrix4.Identity;
        public bool Enabled { get; set; } = false;
        public string Id { get; set; } = "NO_ACTOR_ID";
        
        // A property to be setup once the object has been instanced by the world
        private World? _world;
        public World World { get => _world!; private set => _world = value; }
        
        public Actor()
        {
        }

        public Actor(Actor actor)
        {
            MeshId = actor.MeshId;
            TextureId = actor.TextureId;
            Id = actor.Id;
            Enabled = actor.Enabled;
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


        public virtual void Start()
        {
        }

        public virtual void Update(double deltaTime)
        {
        }

        protected virtual void RenderImGui()
        {
        }
    }
}
