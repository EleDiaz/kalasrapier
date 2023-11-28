namespace Kalasrapier.Engine.Rendering {
    public enum ShaderInfo
    {
        VERTICES = 1,
        COLORS = 2,
        UV = 4,
        NORMALS = 8,
        WEIGHTS = 16,
        TEXTURE = 32,
    }
    
    

    
    public class ShaderManager {
        private Dictionary<MeshInfo, Shader> _shaders;
        
        public ShaderManager() {
            _shaders = new Dictionary<MeshInfo, Shader>();
        }
        
        public void LoadShader(MeshInfo meshInfo, Shader shader) {
            _shaders.Add(meshInfo, shader);           
        }
    }
}