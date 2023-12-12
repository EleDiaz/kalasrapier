using Kalasrapier.Engine.Rendering.Services;

namespace Kalasrapier.Engine.Rendering;

public class RenderPipeline
{
    // For our initial approach we only have 64 possible pipelines actives
    // TODO: Future use?
    public ulong Id { get; set; } = 0;
    // tag to indentify the RenderPipeline in json format
    public virtual string Tag => "NO_PIPELINE";

    protected Shader Shader;
    // By default we don't even request any vertex info data
    public virtual VertexInfo VertexInfo => 0;

    protected RenderPipeline(Shader shader)
    {
        Shader = shader;
    }
    
    public bool BelongsToPipeline(Actor actor)
    {
        return (actor.RenderPipeline & Id) != 0;
    }

    // Given the list of actors subscribed to the pipeline, generated the assets for later usage.
    public virtual void Setup(IEnumerable<Actor> actors)
    {
    }

    public virtual void Render(IEnumerable<Actor> actors)
    {
    }
}