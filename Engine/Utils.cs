using System.Diagnostics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using ErrorCode = OpenTK.Graphics.OpenGL.ErrorCode;

namespace Kalasrapier.Engine;

public static class Utils
{
    public static readonly Vector3 Forward = -Vector3.UnitZ;
    public static readonly Vector3 Right = Vector3.UnitX;
    public static readonly Vector3 Up = Vector3.UnitY;

    public static void CheckGlError(string title)
    {
        ErrorCode error;
        int i = 1;
        while ((error = GL.GetError()) != ErrorCode.NoError)
        {
            Debug.Print($"{title} ({i++}): {error}");
        }
    }
        
        
    // Quite useful when we debug the application through RenderDoc, it requires
    // like 4.3 opengl
    public static void LabelObject(ObjectLabelIdentifier objLabelIdent, int glObject, string name)
    {
        GL.ObjectLabel(objLabelIdent, glObject, name.Length, name);
    }
}