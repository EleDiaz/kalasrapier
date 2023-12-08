using Kalasrapier.Engine.ImportJson;
using Kalasrapier.Engine.Rendering;
using Kalasrapier.Engine.Rendering.Services;
using OpenTK.Graphics.OpenGL4;

namespace Kalasrapier.Game
{
    public class DefaultPipeline : RenderPipeline
    {
        public override string Tag => "DEFAULT_PIPELINE";
        public override VertexInfo VertexInfo => VertexInfo.UV | VertexInfo.NORMALS | VertexInfo.VERTICES;

        const string VertexShader = "Assets/Shaders/Default.vert";
        const string FragmentShader = "Assets/Shaders/Default.frag";

        public DefaultPipeline() :
            base(new Shader(VertexShader, FragmentShader))
        {
        }

        public override void Setup(IEnumerable<Actor> actors)
        {
            foreach (var actor in actors)
            {
                if (actor.MeshId != null) Locator.MeshManager.LoadMesh(actor.MeshId, VertexInfo);

                if (actor.TextureId is not null)
                {
                    Locator.TextureManager.LoadTexture(actor.TextureId);
                }
            }
        }

        public override void Render(IEnumerable<Actor> actors)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            var camera = Locator.ActorManager.GetMainCamera();

            _shader.Use();
            _shader.SetMatrix4("view", camera.GetViewMatrix());
            _shader.SetMatrix4("projection", camera.GetProjectionMatrix());

            foreach (var actor in actors)
            {
                var mesh = Locator.MeshManager.MeshesInfo[actor.MeshId!];
                mesh.SetActiveMesh();
                _shader.SetMatrix4("model", actor.GetWorldTransform());
                var texture = Locator.TextureManager.GetTexture(actor.TextureId!);
                texture.Use(TextureUnit.Texture0);
                // Use the drawing primitives

                if (mesh.Slots is null)
                {
                    if (VertexInfo.HasFlag(VertexInfo.UV))
                    {
                        // With UV our mesh replicate the vertices making the indexArray useless
                        GL.DrawArrays(PrimitiveType.Triangles, 0, mesh.IndicesLenght);
                    }
                    else
                    {
                        // https://docs.gl/gl4/glDrawElements
                        GL.DrawElements(PrimitiveType.Triangles, mesh.IndicesLenght, DrawElementsType.UnsignedInt, 0);
                    }
                }
                else
                {
                    // Make a draw call for each texture color
                    for (int i = 0; i < mesh.Slots.Length; i++)
                    {
                        mesh.Materials![i].SetActive(_shader);
                        GL.DrawElements(PrimitiveType.Triangles, (int)mesh.Slots[i].Offset,
                            DrawElementsType.UnsignedInt,
                            (int)(mesh.Slots[i].Start * sizeof(uint)));
                    }
                }
                Utils.CheckGLError("Draw Mesh: " + actor.MeshId);
            }
        }
    }
}