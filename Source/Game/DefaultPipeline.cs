using Kalasrapier.Engine.ImportJson;
using Kalasrapier.Engine.Rendering;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Kalasrapier.Game
{
    public class DefaultPipeline : RenderPipeline
    {
        public override string TAG => "DEFAULT_PIPELINE";
        public override VertexInfo VertexInfo => VertexInfo.UV | VertexInfo.NORMALS | VertexInfo.VERTICES;

        const string VertexShader = "Assets/Shaders/Default.vert";
        const string FragmentShader = "Assets/Shaders/Default.frag";

        public DefaultPipeline() :
            base(new Shader(VertexShader, FragmentShader))
        {
        }

        public override void Setup(IEnumerable<Actor> actors, MeshManager meshManager, TextureManager textureManager)
        {
            foreach (var actor in actors)
            {
                if (actor.MeshId != null) meshManager.LoadMesh(actor.MeshId, VertexInfo);

                if (actor.TextureId is not null)
                {
                    textureManager.LoadTexture(actor.TextureId);
                }
            }
        }

        public override void Render(IEnumerable<Actor> actors, Camera camera, MeshManager meshManager,
            TextureManager textureManager)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _shader.Use();
            _shader.SetMatrix4("view", camera.GetViewMatrix());
            _shader.SetMatrix4("projection", camera.GetProjectionMatrix());

            foreach (var actor in actors)
            {
                var mesh = meshManager.MeshesInfo[actor.MeshId!];
                mesh.SetActiveMesh();
                _shader.SetMatrix4("model", actor.GetWorldTransform());
                var texture = textureManager.GetTexture(actor.TextureId!);
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
                        GL.DrawElements(PrimitiveType.Triangles, (int)mesh.Slots[i].offset,
                            DrawElementsType.UnsignedInt,
                            (int)(mesh.Slots[i].start * sizeof(uint)));
                    }
                }

                Utils.CheckGLError("Draw Mesh: " + actor.MeshId);
            }
        }
    }
}