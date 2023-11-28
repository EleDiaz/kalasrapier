using OpenTK.Mathematics;
using Kalasrapier.Engine.Rendering;

namespace Kalasrapier.Game
{

    public class Pawn : Actor
    {
        private Camera _camera;
        private Controller _controller;
        private float cameraDistance = 2f;

        public float Speed { get; set; }
        public float MouseSensibility { get; set; }


        public override void Start()
        {
            _camera = World.Camera;
            _controller = new Controller();
            Speed = 1f;
            MouseSensibility = 50f;
            Enabled = true;
            MeshId = "pawn";
            Id = "pawn";
        }

        public override void Update(double deltaTime)
        {
            _controller.UpdateState(World.Window);

            Vector3 movement = _controller.GetMovement() * Speed * (float)deltaTime;
            Vector2 angles = _controller.GetArmDirection();
            
            var rotation = Matrix4.CreateRotationX(angles.X) * Matrix4.CreateRotationY(angles.Y);
            Transform *= rotation * Matrix4.CreateTranslation(movement * Speed * (float)deltaTime);

            _camera.Transform = Matrix4.CreateTranslation(1f, 0, 0) * rotation;
        }
    }
}
