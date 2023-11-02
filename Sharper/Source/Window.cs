using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using ImGuiNET;
using OpenTK.Mathematics;

namespace Kalasrapier
{
    // Be warned, there is a LOT of stuff here. It might seem complicated, but just take it slow and you'll be fine.
    // OpenGL's initial hurdle is quite large, but once you get past that, things will start making more sense.
    public class Window : GameWindow
    {
        ImGuiController? _imGuiController;

        private Shader? _shader;

        private Controller _controller;

        // TODO: Remove
        private Level _level = new Level();

        // TODO: Remove
        private readonly string _levelFilePath = "mesh.json";

        private MeshLoader? _meshLoader;

        private Camera _camera;

        private Matrix4 _model;

        private readonly Vector3 _axis = new Vector3(1f, 1f, 0f);

        private float _rotSpeed = 1f;

        private float _rotAngle;


        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            _camera = new Camera(Vector3.UnitZ * 3, ClientSize.X / (float)ClientSize.Y);
            _model = new Matrix4();
            _rotAngle = 0f;
            _controller = new Controller();
        }

        // TODO: Remove
        protected void InitializeLevel()
        {
            _level=new Level(_levelFilePath);
            _level.LoadLevel();
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
            GL.ProvokingVertex(ProvokingVertexMode.FirstVertexConvention);
            _meshLoader = new MeshLoader("Meshes/cube_colors_flat_shading.json");
            _meshLoader.LoadMeshDSA();

            _shader = new Shader("Shaders/vert.glsl", "Shaders/frag.glsl");

            _shader.Use();

            var posLocation = _shader.GetAttribLocation("aPosition");

        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            _imGuiController?.Update(this, (float)e.Time);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _shader!.Use();

            _shader!.SetMatrix4("model", _model);
            _shader!.SetMatrix4("view", _camera.GetViewMatrix());
            _shader!.SetMatrix4("projection", _camera.GetProjectionMatrix());

            _meshLoader!.SetActiveMesh();
            _meshLoader!.DrawMesh();
            // INFO: para renderizar con varios materiales se utiliza varias llamadas a draw cambiando el parámetro de 
            // diffuse color (uniform) La llamada pinta solo aquellos indices que tengan asignado el material
            // jugando con el offset, ojo puntero al primer indece del array de indices pasar por ref
            // GL.DrawElements(PrimitiveType.Triangles,nelements,DrawElementsType.UnsignedInt, ref indexData[slotData[i]]);


            ImGui.SliderFloat("Rotation Speed", ref _rotSpeed, 0.0f, 10.0f);
            ImGui.SliderAngle("Angle", ref _rotAngle);
            ImGui.SliderAngle("Camera yaw", ref _camera._yaw);

            _imGuiController?.Render();

            Utils.CheckGLError("End of frame");
            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            _controller.UpdateState(this, e);

            Vector3 movement = _controller.GetMovement();
            Vector2 angles = _controller.GetArmDirection();

            // TODO: We are negating the Z axis changing the Opengl forward, there is something weird. See Utils.cs
            // This change could be happening in the projection view?
            _camera.Position += _camera.Front * -movement.Z + _camera.Right * movement.X + _camera.Up * movement.Y;
            // TODO: This generates a problem due to the rotation lock.
            _camera.Yaw += angles.X;
            _camera.Pitch += angles.Y;

            Matrix4.CreateFromAxisAngle(_axis, _rotAngle, out _model);
            _rotAngle += _rotSpeed * (float)e.Time;

            if (_rotAngle >= MathHelper.TwoPi){
                _rotAngle = 0;
            }

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

        // Now, for cleanup.
        // You should generally not do cleanup of opengl resources when exiting an application,
        // as that is handled by the driver and operating system when the application exits.
        //
        // There are reasons to delete opengl resources, but exiting the application is not one of them.
        // This is provided here as a reference on how resource cleanup is done in opengl, but
        // should not be done when exiting the application.
        //
        // Places where cleanup is appropriate would be: to delete textures that are no
        // longer used for whatever reason (e.g. a new scene is loaded that doesn't use a texture).
        // This would free up video ram (VRAM) that can be used for new textures.
        //
        // The coming chapters will not have this code.
        protected override void OnUnload()
        {
            // Unbind all the resources by binding the targets to 0/null.
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            _meshLoader?.Unload();

            // Delete all the resources.
            if (_shader is not null)
            {
                GL.DeleteProgram(_shader.Handle);
            }
            base.OnUnload();
        }
    }
}
