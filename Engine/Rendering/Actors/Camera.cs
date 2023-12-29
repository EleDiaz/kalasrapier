using Kalasrapier.Engine.ImportJson;
using Kalasrapier.Engine.Rendering.Services;
using OpenTK.Mathematics;

namespace Kalasrapier.Engine.Rendering.Actors;

// Do not modified on any kind of update loop the camera rotation or position through Transform component due to
// the requirement of extracting those values int each component (yaw, pitch, roll)
public class Camera : Actor
{
    public static string Tag = "CAMERA";

    // The field of view of the camera (radians)
    private float _fov = MathHelper.PiOver2;
    
    protected Singleton<CameraManager> CameraManager { get; set; }

    public Camera(ActorData template): base(template)
    {
        
    }

    public float AspectRatio { private get; set; }

    public override void Start()
    {
        base.Tag = Tag;
        Enabled = true;
        var ratio = World.Window.AspectRatio ?? (1, 1);
        AspectRatio = (ratio.numerator / (float)ratio.denominator);
    }

    public Matrix4 GetViewMatrix()
    {
        Transform = Transform.ClearScale();
        var viewMatrix = new Matrix4
        {
            // Invert the rotations
            Column0 = new Vector4(Transform.Row0.Xyz),
            Column1 = new Vector4(Transform.Row1.Xyz),
            Column2 = new Vector4(Transform.Row2.Xyz),
            // Invert the translation
            Row3 = new Vector4(-1 * Transform.Row3.Xyz, 1.0f)
        };
        return viewMatrix;
    }

    public Matrix4 GetProjectionMatrix()
    {
        // TODO: Check if this changes the Axis orientation
        return Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, 0.01f, 100f);
    }
}