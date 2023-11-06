using Kalasrapier.Engine.ImportJson;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace Kalasrapier.Engine.Rendering
{
    public class Actor
    {
        public string mesh_id;
        public Matrix4 Transform;
        public bool enabled;
        public string id;

        public Actor() {
            mesh_id = "NO_MESH_ACTOR_ID";
            id = "NO_ACTOR_ID";
            enabled = false;
            Transform = Matrix4.Identity;
            SetupCallbacks();
        }

        public Actor(ActorJson actorJson)
        {
            mesh_id = actorJson.sm;
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
        private void SetupCallbacks() {
            Window.Self!.UpdateFrame += UpdateFrame;
        }

        protected virtual void UpdateFrame(FrameEventArgs e) {
        }
    }
}
