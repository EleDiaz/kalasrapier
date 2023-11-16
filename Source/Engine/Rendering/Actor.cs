using Kalasrapier.Engine.ImportJson;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace Kalasrapier.Engine.Rendering
{
    public class Actor
    {
        // TODO: Shader Reference.
        public string? mesh_id;
        public string? texture_id;
        public Matrix4 Transform;
        public bool enabled;
        public string id;

        public Actor()
        {
            id = "NO_ACTOR_ID";
            enabled = false;
            Transform = Matrix4.Identity;
            SetupCallbacks();
        }

        public Actor(Actor actor) {
            mesh_id = actor.mesh_id;
            texture_id = actor.texture_id;
            id = actor.id;
            enabled = actor.enabled;
        }

        public Actor(ActorJson actorJson)
        {
            mesh_id = actorJson.mesh_id;
            texture_id = actorJson.texture_id;
            id = actorJson.id;
            enabled = actorJson.enabled;

            var scale = new Vector3(actorJson.scale[0], actorJson.scale[1], actorJson.scale[2]);
            var axis = new Vector3(actorJson.orientation.axis[0], actorJson.orientation.axis[1], actorJson.orientation.axis[2]);
            var position = new Vector3(actorJson.position[0], actorJson.position[1], actorJson.position[2]);

            Transform = Matrix4.CreateScale(scale) * Matrix4.CreateFromAxisAngle(axis, actorJson.orientation.angle) * Matrix4.CreateTranslation(position);
            SetupCallbacks();
        }

        // We could setup more callbacks like the render one. But, things could turn wild, without a clear access to
        // the camera draw call order.
        private void SetupCallbacks()
        {
            Window.Self!.UpdateFrame += UpdateFrame;
            Window.Self!.RenderGUI += RenderGUI;
        }

        protected virtual void UpdateFrame(FrameEventArgs e)
        {
        }

        protected virtual void RenderGUI()
        {
        }
    }
}
