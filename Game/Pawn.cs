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
        private float _cameraDistance = 4f;

        private Quaternion _yawRotation = Quaternion.Identity;
        private Quaternion _pitchRotation = Quaternion.Identity;
        private Quaternion _initialRotation = Quaternion.Identity;
        private BoxCollider? _boxCollider;
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
            Speed = 10f;
            _controller.Speed = Speed;
            Enabled = true;
            _currentPosition = Transform.ExtractTranslation();
            _initialRotation = Transform.ExtractRotation();
            _boxCollider = GetComponent<BoxCollider>() as BoxCollider;
        }

        public override void Update(double deltaTime)
        {
            _controller!.UpdateState(Director.Window);

            var movement = _controller.GetMovement();
            // _currentPosition += (float)deltaTime * (_currentRotation * Utils.Forward * -movement.Z + _currentRotation * Utils.Right * movement.X + _currentRotation * Utils.Up * movement.Y);
            _currentPosition += (float)deltaTime * (_yawRotation * Utils.Forward * -movement.Z);

            var angles = _controller.GetArmDirection() / 500f;

            _yawRotation *= Quaternion.FromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(angles.X));
            _pitchRotation *= Quaternion.FromAxisAngle(Vector3.UnitX, MathHelper.DegreesToRadians(angles.Y));
            var rotation = _yawRotation * _initialRotation * _pitchRotation;
            var rot = Matrix4.CreateFromQuaternion(_yawRotation);

            Transform = rot * Matrix4.CreateTranslation(_currentPosition);

            var cameraPos = _currentPosition + rotation * new Vector3(0, 0, _cameraDistance);
            _camera!.Actor.Transform = Matrix4.CreateFromQuaternion(rotation) * Matrix4.CreateTranslation(cameraPos);
        }

        public override void OnTriggerEnter(Actor actor)
        {
            if (actor.Tag == "mark_trigger")
            {
                Console.WriteLine("Reached");
            }

            // The idea would be to implement a FixedUpdate, with its rigidbody fully controlled with physics
            // But! For the time being, we just go by: If you are confident enough, you can go through walls.
            _controller!.Speed = 0.5f;
        }

        public override void OnTriggerStay(Actor actor)
        {
        }

        public override void OnTriggerExit(Actor actor)
        {
            _controller!.Speed = Speed;
        }

    }
}
