using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using Kalasrapier.Engine.ImportJson;
using Kalasrapier.Engine.Rendering;
using Kalasrapier.Engine;

namespace Kalasrapier.Game
{

    public class Pawn : Actor
    {
        private Camera _camera;
        private Vector3 _movementInput = new Vector3();
        private Vector2 _armAngles = new Vector2();
        private bool _active = false;
        private Controller _controller;

        public float Speed { get; set; }
        public float MouseSensibility { get; set; }


        public Pawn(ActorJson actorJson) : base(actorJson)
        {
            // TODO: This should have some kind of system to keep a set of camera, 
            //       and which of them is active.
            _camera = Window.Self!.Camera;
            _controller = new Controller();
            Speed = 1f;
            MouseSensibility = 50f;
        }


        protected override void UpdateFrame(FrameEventArgs e)
        {
            _controller.UpdateState(Window.Self!, e);

            Vector3 movement = _controller.GetMovement();
            Vector2 angles = _controller.GetArmDirection();

            // TODO: We are negating the Z axis changing the Opengl forward, there is something weird. See Utils.cs
            // This change could be happening in the projection view?
            _camera.Position += _camera.Front * -movement.Z + _camera.Right * movement.X + _camera.Up * movement.Y;
            // TODO: This generates a problem due to the rotation lock.
            _camera.Yaw += angles.X;
            _camera.Pitch += angles.Y;
        }
    }
}
