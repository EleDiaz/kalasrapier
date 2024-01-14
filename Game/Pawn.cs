using Kalasrapier.Engine;
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
        private float _cameraDistance = 10f;

        private Quaternion _yaw = Quaternion.Identity;
        private Quaternion _pitch = Quaternion.Identity;
        private Quaternion _currentRotation = Quaternion.Identity;
        private Vector3 _currentPosition = Vector3.Zero;

        private float Speed { get; set; }

        public Pawn(Director director) : base(director)
        {
            var pawnData = Director.ActorManager.FindTemplate("pawn");
            ImportTemplate(pawnData);
        }

        public override void Start()
        {
            _camera = Director.Cameras.ActiveCamera;
            _controller = new Controller();
            Speed = 1f;
            Enabled = true;
            _currentPosition = Transform.ExtractTranslation();
        }

        public override void Update(double deltaTime)
        {
            _controller!.UpdateState(Director.Window);

            var movement = _controller.GetMovement();
            _currentPosition += (float)deltaTime * (_currentRotation * Utils.Forward * -movement.Z + _currentRotation * Utils.Right * movement.X + _currentRotation * Utils.Up * movement.Y);
            var angles = _controller.GetArmDirection() / 500f;

            _yaw = Quaternion.FromAxisAngle(Vector3.UnitX, MathHelper.DegreesToRadians(angles.Y));
            _pitch = Quaternion.FromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(angles.X));
            _currentRotation = _pitch * _currentRotation * _yaw;
            var rot = Matrix4.CreateFromQuaternion(_currentRotation);

            Transform = rot * Matrix4.CreateTranslation(_currentPosition);

            var cameraPos = _currentPosition + _currentRotation * new Vector3(0, 0, _cameraDistance);
            _camera!.Actor.Transform = rot * Matrix4.CreateTranslation(cameraPos);
        }
    }
}
