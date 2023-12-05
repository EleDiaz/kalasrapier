using Kalasrapier.Engine.ImportJson;

namespace Kalasrapier.Engine.Rendering
{
    public class RenderPipeline
    {
        // For our initial approach we only have 64 possible pipelines actives
        public ulong Id { get; set; } = 0;
        // tag to indentify the RenderPipeline in json format
        public virtual string TAG {get { return "NO_PIPELINE"; }}

        protected Shader _shader;
        // By default we don't even request any vertex info data
        public virtual VertexInfo VertexInfo => 0;

        public RenderPipeline(Shader shader)
        {
            _shader = shader;
        }

        // Given the list of actors subscribed to the pipeline, generated the assets for later usage.
        public virtual void Setup(IEnumerable<Actor> actors, MeshManager meshManager, TextureManager textureManager)
        {
        }

        public virtual void Render(IEnumerable<Actor> actors, Camera camera, MeshManager meshManager, TextureManager textureManager)
        {
        }
    }
}