using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Kalasrapier
{
    public interface IController
    {
        // Recieve information to the controller
        void UpdateState(Window window, FrameEventArgs eventArgs);
        Vector3 GetMovement();
        Vector2 GetArmDirection();
    }

    public class Controller : IController
    {
        private FrameEventArgs _frameEvents;
        // TODO: Vector2
        private Vector3 _movementInput = new Vector3();
        private Vector2 _armAngles = new Vector2();
        private float _deltaTime;
        private bool _active = false;

        public float Speed { get; set; }
        public float MouseSensibility { get; set; }

        public Controller()
        {
            Speed = 1f;
            MouseSensibility = 50f;
        }

        public Vector2 GetArmDirection()
        {
            return _armAngles * MouseSensibility * _deltaTime;
        }

        public Vector3 GetMovement()
        {
            return _movementInput * Speed * _deltaTime;
        }

        public void UpdateState(Window window, FrameEventArgs eventArgs)
        {
            var keyboardState = window.KeyboardState;
            var mouseState = window.MouseState;
            _frameEvents = eventArgs;
            _deltaTime = (float)eventArgs.Time;
            _movementInput = Vector3.Zero;
            _armAngles = Vector2.Zero;

            if (keyboardState.IsKeyPressed(Keys.Space)) {
                if (window.CursorState == CursorState.Normal) {
                    window.CursorState = CursorState.Grabbed;
                    _active = true;
                }
                else {
                    window.CursorState = CursorState.Normal;
                    _active = false;
                }
            }

            if (!_active)
                return;

            if (keyboardState.IsKeyDown(Keys.W))
                _movementInput = Utils.FORWARD;
            if (keyboardState.IsKeyDown(Keys.S))
                _movementInput = -Utils.FORWARD;
            if (keyboardState.IsKeyDown(Keys.A))
                _movementInput = -Utils.RIGHT;
            if (keyboardState.IsKeyDown(Keys.D))
                _movementInput = Utils.RIGHT;
            if (keyboardState.IsKeyDown(Keys.E))
                _movementInput = Utils.UP;
            if (keyboardState.IsKeyDown(Keys.Q))
                _movementInput = -Utils.UP;

            _armAngles = new Vector2(mouseState.Delta.X, -mouseState.Delta.Y);
        }
    }
}