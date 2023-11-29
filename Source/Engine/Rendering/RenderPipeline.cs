using OpenTK.Mathematics;

namespace Kalasrapier.Engine.Rendering
{
    public class RenderPipeline
    {
        private Shader _shader;
        public MeshInfo MeshInfo { get; set; } = new();
        public Dictionary<string, Type> Uniforms { get; set; } = new();
        
        public RenderPipeline(Shader shader)
        {
            _shader = shader;
        }

        public void AddInput(MeshInfo meshInfo)
        {
            MeshInfo = meshInfo;
        }

        public void AddUniform<T>(string name)
        {
            Uniforms.Add(name, typeof(T));
        }

        public virtual void Render(IEnumerable<Actor> actors)
        {
        }
    }
}