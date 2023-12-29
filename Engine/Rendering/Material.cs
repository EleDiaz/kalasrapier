using Kalasrapier.Engine.ImportJson;
using OpenTK.Mathematics;

namespace Kalasrapier.Engine.Rendering;

// Later this class will in charge of creating the texture sampler for the shader
public class Material
{
    public string Name;
    public float[] DiffuseColor;

    public Material(MaterialData materialData)
    {
        DiffuseColor = materialData.DiffuseColor;
        Name = materialData.Name;
    }

    // TODO: Remove from here
    public void SetActive(Shader shader)
    {
        shader.SetVector3("diffuse_color", new Vector3(DiffuseColor[0], DiffuseColor[1], DiffuseColor[2]));
    }
}