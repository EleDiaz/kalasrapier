using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using ImGuiNET;
using OpenTK.Mathematics;
using Kalasrapier.Engine.Rendering;
using Kalasrapier.Game;

namespace Kalasrapier.Engine
{
    public class WindowOld : GameWindow
    {
        // TODO: Initial Approach
        public delegate void OnRenderGUI();
        public event OnRenderGUI RenderGUI;

        private ImGuiController? _imGuiController;
        private Scene? _scene;
        private Shader? _shader;
        public CameraOld Camera;

        // TODO: We need a singleton, this is a bit hacky way but no that far from a better solution.
        // SAFETY: Window will live longer than any actor, pawn in our framework. unless we start playing with several
        // windows, which is out of scope.
        public static WindowOld Self;

        public WindowOld(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            Self = this;
            var ratio = Window.Self?.AspectRatio ?? (1, 1);
            Camera = new CameraOld(ratio.numerator / (float)ratio.denominator);
        }

        // Now, we start initializing OpenGL.
        protected override void OnLoad()
        {
            base.OnLoad();
            _imGuiController = new ImGuiController(ClientSize.X, ClientSize.Y);

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);

            // Decide which vertex will be used as ruler when interpolation is set to flat
            // GL.ProvokingVertex(ProvokingVertexMode.FirstVertexConvention);
            _scene = new Scene("Scenes/simple.json");

            // _shader = new Shader("Shaders/vert.glsl", "Shaders/frag.glsl");
            _shader = new Shader("Shaders/material_vert.glsl", "Shaders/material_frag.glsl");
            // TODO: no using scene properties
            var pawn = new Pawn();
            _scene.Actors[pawn.Id] = pawn;

            _shader.Use();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            _imGuiController?.Update(this, (float)e.Time);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // TODO: options
            // - Shader class only storing the available shaders 
            //   - each actor decides which shader will use
            //     - It would imply an inefficient draw call, with many context switches 
            //       - solution, grouping those meshes into those shaders.
            //       - Shader class will have cache storing those groups (more complexity)
            _shader!.Use();

            foreach (var actor in _scene!.Actors.Values)
            {
                if (actor.TextureId is not null) {
                    _scene.Textures[actor.TextureId].Use(TextureUnit.Texture0);
                }
                if (actor.MeshId is not null)
                {
                    _shader!.SetMatrix4("model", actor.Transform);
                    _shader!.SetMatrix4("view", Camera.GetViewMatrix());
                    _shader!.SetMatrix4("projection", Camera.GetProjectionMatrix());
                    var mesh = _scene.Meshes.MeshesInfo[actor.MeshId];
                    mesh.SetActiveMesh();
                    mesh.DrawMesh(_shader);
                }
            }

            // ImGui.SliderAngle("Angle", ref _rotAngle);
            // ImGui.SliderAngle("Camera yaw", ref _camera._yaw);

            // Render sub ImGUI elements
            RenderGUI();

            _imGuiController?.Render();

            Utils.CheckGLError("End of frame");
            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            var input = KeyboardState;
            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            // When the window gets resized, we have to call GL.Viewport to resize OpenGL's viewport to match the new size.
            // If we don't, the NDC will no longer be correct.
            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);

            _imGuiController?.WindowResized(ClientSize.X, ClientSize.Y);
        }

        protected override void OnTextInput(TextInputEventArgs e)
        {
            base.OnTextInput(e);


            _imGuiController?.PressChar((char)e.Unicode);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            _imGuiController?.MouseScroll(e.Offset);
        }

        protected override void OnUnload()
        {
            // Unbind all the resources by binding the targets to 0/null.
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            // _scene?.Unload();

            // Delete all the resources.
            if (_shader is not null)
            {
                GL.DeleteProgram(_shader.Handle);
            }
            base.OnUnload();
        }
    }
}
