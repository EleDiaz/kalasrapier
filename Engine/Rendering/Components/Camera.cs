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

    // Let's talk:
    // OpenTK Uses a ROW-MAJOR, to store those matrices, this has a lot of implications
    // if you mixed with COLUMN-MAJOR.
    // What implies those row-major?
    // - different order of multiplication. instead of the TRS this becomes SRT
    //
    // Then the View Matrix, is a conversion from world to camera being W the Camera to world we need its inverse.
    // W? => RT
    // W^-1 => T^-1 * R^-1
    // Order is changed!
    // so ...... 
    // T^-1 is just multiply per -1
    // R^-1 is the transpose
    // then multiply those
    public Matrix4 GetViewMatrix()
    {
        var transform = Actor.GetWorldTransform().ClearScale();
        var q0 = Vector4.Dot(transform.Row3, transform.Row0);
        var q1 = Vector4.Dot(transform.Row3, transform.Row1);
        var q2 = Vector4.Dot(transform.Row3, transform.Row2);
        var viewMatrix = new Matrix4
        {
            // Invert the rotations
            Column0 = new Vector4(transform.Row0.Xyz),
            Column1 = new Vector4(transform.Row1.Xyz),
            Column2 = new Vector4(transform.Row2.Xyz),
            // Invert the translation
            Row3 = new Vector4(-q0, -q1, -q2, 1.0f)
        };
        return viewMatrix;

        // Another way is just to call the invert, safe bet
        // return transform.Inverted();
    }

    public Matrix4 GetProjectionMatrix()
    {
        return Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, 0.01f, 100f);
    }

    public override void Destroy()
    {
        Actor.Director.Cameras.UnRegisterCamera(this);
        base.Destroy();
    }
}