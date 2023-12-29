using Kalasrapier.Engine.Rendering;
using Kalasrapier.Engine.Rendering.Services;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Kalasrapier.Engine;

public class GameManager
{
    private GameWindow? _window;
    private World? _world;
    private ImGuiController? _imGuiController;

    public GameManager()
    {
        BuildWindow();
        if (_window != null)
        {
            _world = new World(_window);
            Base.SetDefaultsBaseWithNewWorld(_world);
        }
    }

    public void Run()
    {
        using (_window)
        {
            _window!.Run();
        }
    }

    private void BuildWindow()
    {
        // todo: make external
        var nativeWindowSettings = new NativeWindowSettings
        {
            API = ContextAPI.OpenGL,
            APIVersion = new Version(4, 6),
            Size = new Vector2i(800, 600),
            Title = "Kala's Rapier",
            // This is needed to run on macos
            Flags = ContextFlags.ForwardCompatible,
        };
        _window = new GameWindow(GameWindowSettings.Default, nativeWindowSettings);

        _window.Load += WindowOnLoad;
        _window.Unload += WindowOnUnload;
        _window.RenderFrame += WindowOnRenderFrame;
        _window.UpdateFrame += WindowOnUpdateFrame;
        _window.Resize += WindowOnResize;
        _window.TextInput += WindowOnTextInput;
        _window.MouseWheel += WindowOnMouseWheel;
    }

    private void WindowOnUnload()
    {
        // Unbind all the resources by binding the targets to 0/null.
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
        GL.UseProgram(0);
    }

    private void WindowOnMouseWheel(MouseWheelEventArgs e)
    {
        _imGuiController?.MouseScroll(e.Offset);
    }

    private void WindowOnTextInput(TextInputEventArgs e)
    {
        _imGuiController?.PressChar((char)e.Unicode);
    }

    private void WindowOnResize(ResizeEventArgs obj)
    {
        // When the window gets resized, we have to call GL.Viewport to resize OpenGL's viewport to match the new size.
        // If we don't, the NDC will no longer be correct.
        GL.Viewport(0, 0, _window!.ClientSize.X, _window!.ClientSize.Y);

        _imGuiController?.WindowResized(_window!.ClientSize.X, _window!.ClientSize.Y);
    }

    private void WindowOnUpdateFrame(FrameEventArgs obj)
    {
        var input = _window!.KeyboardState;
        if (input.IsKeyDown(Keys.Escape))
        {
            _window.Close();
        }

        _world!.Update(obj.Time);
    }

    private void WindowOnLoad()
    {
        _imGuiController = new ImGuiController(_window!.ClientSize.X, _window!.ClientSize.Y);

        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        GL.Enable(EnableCap.CullFace);
        GL.Enable(EnableCap.DepthTest);
        Preload(_world!);
        _world!.Start();
    }


    private void WindowOnRenderFrame(FrameEventArgs e)
    {
        _imGuiController?.Update(_window!, (float)e.Time);

        // Render
        _world!.Render();

        _imGuiController?.Render();
        Utils.CheckGlError("End of frame");
        _window!.SwapBuffers();
    }

    /// <summary>
    /// Thing to do before the world starts. Loading scenes, render pipelines, extended actors, etc.
    /// </summary>
    /// <param name="world"></param>
    protected virtual void Preload(World world)
    {
    }
}