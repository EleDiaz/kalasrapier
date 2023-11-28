using OpenTK.Mathematics;

namespace Kalasrapier.Engine.Rendering
{
    
    public class Pipeline
    {
        private Shader _shader;
        public MeshInfo MeshInfo { get; set; } = new();
        public Dictionary<string, Type> Uniforms { get; set; } = new();
        
        public Pipeline(Shader shader)
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
        
        public void AddUniformInput(string name, Vector3 vector)
        {
            _shader.SetVector3(name, vector);
        }
        
        public void AddUniformInput(string name, float value)
        {
            _shader.SetFloat(name, value);
        }
        
        public void AddUniformInput(string name, int value)
        {
            _shader.SetInt(name, value);
        }

        public void Render(World world)
        {
        }
    }

}