using Kalasrapier.Engine.Rendering.Actors;
using OpenTK.Mathematics;

namespace Kalasrapier.Engine.Rendering.Components;

public class CameraData : ComponentData
{
    public override Component BuildComponent(Actor actor)
    {
        return new Camera(actor);
    }
}


public class Camera : Component
{
    // The field of view of the camera (radians)
    private readonly float _fov = MathHelper.PiOver2;
    private float AspectRatio { get; }

    public Camera(Actor actor) : base(actor)
    {
        Actor.Director.Cameras.RegisterCamera(this);

        var ratio = Actor.Director.Window.AspectRatio ?? (1, 1);
        AspectRatio = (ratio.numerator / (float)ratio.denominator);
    }

    public Matrix4 GetViewMatrix()
    {
        // TODO: Check if this produces the View Matrix we want
        Actor.Transform = Actor.Transform.ClearScale();
        var viewMatrix = new Matrix4
        {
            // Invert the rotations
            Column0 = new Vector4(Actor.Transform.Row0.Xyz),
            Column1 = new Vector4(Actor.Transform.Row1.Xyz),
            Column2 = new Vector4(Actor.Transform.Row2.Xyz),
            // Invert the translation
            Row3 = new Vector4(-1 * Actor.Transform.Row3.Xyz, 1.0f)
        };
        return viewMatrix;
    }

    public Matrix4 GetProjectionMatrix()
    {
        // TODO: Check if this changes the Axis orientation
        return Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, 0.01f, 100f);
    }

    public override void Destroy()
    {
        Actor.Director.Cameras.UnRegisterCamera(this);
        base.Destroy();
    }
}