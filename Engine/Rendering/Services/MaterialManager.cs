using Kalasrapier.Engine.Rendering.Components;

namespace Kalasrapier.Engine.Rendering.Services;

/// <summary>
/// A Helper class to manage textures
/// </summary>
public class MaterialManager
{
    private Dictionary<string, MaterialData> _materialDatas = new();
    private Dictionary<string, string> _paths = new();
    private Dictionary<string, Texture> _textures = new();
    
    public void AddMaterial(string id, MaterialData materialData)
    {
        _materialDatas.Add(id, materialData);
    }
    
    public MaterialData GetMaterial(string id)
    {
        return _materialDatas[id];
    }
        
    /// <summary>
    /// Add a texture to the manager, *doesn't* load it to the GPU.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="path"></param>
    public void AddTexture(string id, string path)
    {
        _paths.Add(id, path);
    }

    /// <summary>
    /// Load a texture to the GPU.
    /// </summary>
    /// <param name="id"></param>
    public void LoadTexture(string id)
    {
        if (_textures.ContainsKey(id))
        {
            return;
        }
        _textures.Add(id, Texture.LoadFromFile(_paths[id]));
    }

    /// <summary>
    /// Retrieve a texture from the manager.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Texture GetTexture(string id)
    {
        return _textures[id];
    }
}