
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Kalasrapier {
    public interface IController {
        // Recieve information to the controller
        void UpdateState(KeyboardState keyboardState, MouseState mouseState, FrameEventArgs eventArgs);
        Vector3 GetMovement();
        Vector2 GetArmDirection();


    }

    public class Controller : IController
    {
        private readonly Vector3 FORWARD = -Vector3.UnitZ;
        private readonly Vector3 RIGHT = Vector3.UnitX;
        private readonly Vector3 UP = Vector3.UnitY;

        private KeyboardState? _keyboardState;
        private MouseState? _mouseState;
        private float _deltaTime;
        private float _speed = 1.0f;
        private FrameEventArgs _frameEvents;
        private Vector3 _movementInput = new Vector3();
        private Vector2 _armAngles = new Vector2();

        public Vector2 GetArmDirection()
        {
            return _armAngles;
        }

        public Vector3 GetMovement()
        {
            return _movementInput; // Scale
        }

        public void UpdateState(KeyboardState keyboardState, MouseState mouseState, FrameEventArgs eventArgs)
        {
            _keyboardState = keyboardState;
            _mouseState = mouseState;
            _frameEvents = eventArgs;
            _deltaTime = (float)eventArgs.Time;
            _movementInput = Vector3.Zero;
            _armAngles = Vector2.Zero;

            if (keyboardState.IsKeyDown(Keys.W)) {
                _movementInput = FORWARD * _speed * _deltaTime;
            }

        if(_keyboardState.IsKeyDown(Keys.W))
            _movementInput=FORWARD*_speed*_deltaTime;
        if(_keyboardState.IsKeyDown(Keys.S))
            _movementInput= -FORWARD*_speed*_deltaTime;
        if(_keyboardState.IsKeyDown(Keys.A))
            _movementInput=-RIGHT*_speed*_deltaTime;
        if(_keyboardState.IsKeyDown(Keys.D))
            _movementInput=RIGHT*_speed*_deltaTime;
        if(_keyboardState.IsKeyDown(Keys.E))
            _movementInput=UP*_speed*_deltaTime;
        if(_keyboardState.IsKeyDown(Keys.Q))
            _movementInput=-UP*_speed*_deltaTime;

        }
    }
}