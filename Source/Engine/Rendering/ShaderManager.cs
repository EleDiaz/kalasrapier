
namespace Kalasrapier.Engine.Rendering {

    // 
    public class ShaderManager {
        private Dictionary<ShaderID, MeshID> _groups;

        private void GroupingMeshes() {
            // Get an iterator of active meshes,
            // groups them

        }

        private void DrawByShader(ShaderID) {
            // here we just call the mesh draw
            // and if it was possible the Meshes should be able to operate a multidraw call given the
            // meshes id 


            // also remember that each actor needs to update the shader in some minor way, something that could dissappear with the multidraw

        }
    }
}