
using Kalasrapier.Engine.ImportJson;
using OpenTK.Mathematics;

namespace Kalasrapier.Engine.Rendering
{
    // Later this class will in charge of creating the texture sampler for the shader
    public class Material
    {
        public string name;
        public float[] diffuseColor;

        public Material(MaterialJson materialJson) {
            diffuseColor = materialJson.DiffuseColor;
            name = materialJson.Name;
        }

        public void SetActive(Shader shader)
        {
            shader.SetVector3("diffuse_color", new Vector3(diffuseColor[0], diffuseColor[1], diffuseColor[2]));
        }
    }
}
