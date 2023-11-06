using Kalasrapier.Engine;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Window = Kalasrapier.Engine.Window;

namespace Kalasrapier.Game
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
        private Vector3 _movementInput = new Vector3();
        private Vector2 _armAngles = new Vector2();

        // private Angles2D _armAnglesTarge = new Angles2D();

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
            //window.ClientSize
            var keyboardState = window.KeyboardState;
            var mouseState = window.MouseState;
            _frameEvents = eventArgs;
            _deltaTime = (float)eventArgs.Time;
            _movementInput = Vector3.Zero;
            _armAngles = Vector2.Zero;

            if (keyboardState.IsKeyPressed(Keys.Space))
            {
                if (window.CursorState == CursorState.Normal)
                {
                    window.CursorState = CursorState.Grabbed;
                    _active = true;
                }
                else
                {
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

            // This can be modified to keep some relation to the windows size.
            // For example: a 8k windows will its mouse have bigger delta?
            // In that case we would need to adjust the sensibility due to this parameter.
            _armAngles = new Vector2(mouseState.Delta.X, -mouseState.Delta.Y);

            /*
            if (_mouseState.IsButtonDown(MouseButton.Left))
            {
                float deltaX, deltaY;
                // Normalize to device
                float mx = (float)_mouseState.X / _hRes;
                float my = (float)_mouseState.Y / _vRes;

                if (_firstMouse)
                {
                    _lastMouse = new Vector2(mx, my);
                    _firstMouse = false;
                }
                else
                {
                    deltaX = _lastMouse.X - mx;
                    deltaY = _lastMouse.Y - my;

                    _lastMouse = new Vector2(mx, my);
                    _armAnglesTarget = _armAnglesTarget + new Angles2D(deltaX, deltaY) * _deltaTime * MouseSensitivity;

                    _armAngles = _armAngles + ArmRate * (_armAnglesTarget - _armAngles);
                }
            }
            else
                _firstMouse = true;
            */

        }
    }
}