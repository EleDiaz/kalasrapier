using Kalasrapier.Engine.Rendering;
using OpenTK.Mathematics;

namespace Kalasrapier.Game
{

    public class Pawn : Actor
    {
        private Camera? _camera;
        private Controller? _controller;
        private float _cameraDistance = 2f;

        private float Speed { get; set; }


        public override void Start()
        {
            _camera = ActorManager.GetMainCamera();
            _controller = new Controller();
            Speed = 1f;
            Enabled = true;
            MeshId = "pawn";
            Tag = "pawn";

            ActorManager.FindActorsByTag("");
        }

        public override void Update(double deltaTime)
        {
            _controller!.UpdateState(World.Window);

            var movement = _controller.GetMovement() * Speed * (float)deltaTime;
            var angles = _controller.GetArmDirection();
            
            var rotation = Matrix4.CreateRotationX(angles.X) * Matrix4.CreateRotationY(angles.Y);
            Transform *= rotation * Matrix4.CreateTranslation(movement * Speed * (float)deltaTime);

            _camera!.Transform = Matrix4.CreateTranslation(_cameraDistance, 0, 0) * rotation;
        }
    }
}
