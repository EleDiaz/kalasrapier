using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Kalasrapier
{
    public static class Program
    {
        private static void Main()
        {

            Console.WriteLine("????");
            var nativeWindowSettings = new NativeWindowSettings()
            {
               API = ContextAPI.OpenGL,
               APIVersion = new Version(4, 6),
                Size = new Vector2i(800, 600),
                Title = "Kalasrapier",
                // This is needed to run on macos
                Flags = ContextFlags.ForwardCompatible,
            };


            using (var window = new Window(GameWindowSettings.Default, nativeWindowSettings))
            {
                window.Run();
            }
        }
    }
}
