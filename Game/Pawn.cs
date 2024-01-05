using Kalasrapier.Engine.Rendering;
using Kalasrapier.Engine.Rendering.Actors;
using Kalasrapier.Engine.Rendering.Components;
using Kalasrapier.Engine.Rendering.Services;
using OpenTK.Mathematics;

namespace Kalasrapier.Game
{

    public class Pawn : Actor
    {
        private Camera? _camera;
        private Controller? _controller;
        private float _cameraDistance = 2f;

        private float Speed { get; set; }

        public Pawn(Director director) : base(director)
        {
        }

        public override void Start()
        {
            var pawnData = Director.ActorManager.FindTemplate("pawn");
            ImportTemplate(pawnData);

            _camera = Director.Cameras.ActiveCamera;
            _controller = new Controller();
            Speed = 1f;
            Enabled = true;
        }

        public override void Update(double deltaTime)
        {
            _controller!.UpdateState(Director.Window);

            var movement = _controller.GetMovement() * Speed * (float)deltaTime;
            var angles = _controller.GetArmDirection();

            var rotation = Matrix4.CreateRotationX(angles.X) * Matrix4.CreateRotationY(angles.Y);
            Transform *= rotation * Matrix4.CreateTranslation(movement * Speed * (float)deltaTime);

            _camera!.Actor.Transform = Matrix4.CreateTranslation(_cameraDistance, 0, 0) * rotation;
        }
    }
}
