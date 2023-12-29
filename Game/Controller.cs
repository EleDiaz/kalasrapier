using Kalasrapier.Engine;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Kalasrapier.Game
{
    public interface IController
    {
        // Receive information to the controller
        void UpdateState(GameWindow window);
        Vector3 GetMovement();
        Vector2 GetArmDirection();
    }

    public class Controller : IController
    {
        private Vector3 _movementInput;
        private Vector2 _armAngles;

        private bool _active;

        public float Speed { get; set; }
        public float MouseSensibility { get; set; }

        public Controller()
        {
            Speed = 1f;
            MouseSensibility = 50f;
        }

        public Vector2 GetArmDirection()
        {
            return _armAngles * MouseSensibility;
        }

        public Vector3 GetMovement()
        {
            return _movementInput * Speed;
        }

        public void UpdateState(GameWindow window)
        {
            //window.ClientSize
            var keyboardState = window.KeyboardState;
            var mouseState = window.MouseState;
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
                _movementInput = Utils.Forward;
            if (keyboardState.IsKeyDown(Keys.S))
                _movementInput = -Utils.Forward;
            if (keyboardState.IsKeyDown(Keys.A))
                _movementInput = -Utils.Right;
            if (keyboardState.IsKeyDown(Keys.D))
                _movementInput = Utils.Right;
            if (keyboardState.IsKeyDown(Keys.E))
                _movementInput = Utils.Up;
            if (keyboardState.IsKeyDown(Keys.Q))
                _movementInput = -Utils.Up;

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