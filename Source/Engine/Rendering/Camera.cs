using OpenTK.Mathematics;

namespace Kalasrapier.Engine.Rendering
{
    // Do not modified on any kind of update loop the camera rotation or position through Transform component due to
    // the requirement of extracting those values int each component (yaw, pitch, roll)
    public class Camera: Actor
    {
        static public string TAG = "CAMERA";

        // The field of view of the camera (radians)
        private float _fov = MathHelper.PiOver2;

        public float AspectRatio { private get; set; }

        public Quaternion YawRotation = Quaternion.Identity;
        public Quaternion PitchRotation = Quaternion.Identity;
        public Quaternion RollRotation = Quaternion.Identity;

        private Quaternion _currentRotation = Quaternion.Identity;

        public override void Start() {
            Id = TAG;
            Enabled = true;
            var angles = Transform.ExtractRotation().ToEulerAngles();
            Pitch(angles.X);
            Yaw(angles.Y);
            Roll(angles.Z);
            // The order is sooo important hopefully this will behave as i expect
            _currentRotation = YawRotation * PitchRotation * RollRotation;
            var ratio = World.Window.AspectRatio ?? (1, 1);
            AspectRatio = (ratio.numerator / (float)ratio.denominator);
        }

        protected override void RenderImGui()
        {
            // ImGui.Begin("Camera Settings");
            // var yawRotationX = YawRotation.X;
            // ImGui.InputFloat("X: ", ref yawRotationX);
            // ImGui.InputScalarN("", ImGuiDataType.Double, YawRotation, 3);
        }

        public Matrix4 GetViewMatrix()
        {
            var position = Transform.ExtractTranslation();
            var forward = _currentRotation * Vector3.UnitZ;
            var up = _currentRotation * Vector3.UnitY;
            return Matrix4.LookAt(position, position + forward, up);
        }

        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, 0.01f, 100f);
        }

        public void Yaw(float value) {
            YawRotation = Quaternion.FromAxisAngle(Vector3.UnitY, value);
        }
        public void Pitch(float value) {
            PitchRotation = Quaternion.FromAxisAngle(Vector3.UnitX, value);
        }
        public void Roll(float value) {
            RollRotation = Quaternion.FromAxisAngle(Vector3.UnitZ, value);
        }
    }
}