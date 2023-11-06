using System.Diagnostics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using ErrorCode = OpenTK.Graphics.OpenGL.ErrorCode;

namespace Kalasrapier
{
    public static class Utils
    {
        public static readonly Vector3 FORWARD = -Vector3.UnitZ;
        public static readonly Vector3 RIGHT = Vector3.UnitX;
        public static readonly Vector3 UP = Vector3.UnitY;

        public static void CheckGLError(string title)
        {
            ErrorCode error;
            int i = 1;
            while ((error = GL.GetError()) != ErrorCode.NoError)
            {
                Debug.Print($"{title} ({i++}): {error}");
            }
        }
        
        
        // Quite useful when we debug the application throught RenderDoc, it requires
        // like 4.3 opengl
        public static void LabelObject(ObjectLabelIdentifier objLabelIdent, int glObject, string name)
        {
            GL.ObjectLabel(objLabelIdent, glObject, name.Length, name);
        }
    }
}
