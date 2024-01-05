using Kalasrapier.Engine.Rendering.Actors;
using Kalasrapier.Engine.Rendering.Components;
using Kalasrapier.Engine.Rendering.Services;
using Kalasrapier.Engine.Rendering.Services.MeshManager;

namespace Kalasrapier.Engine.Rendering;

public class RenderPipeline
{
    protected Shader Shader;

    // By default we don't even request any vertex info data
    public virtual VertexInfo VertexInfo => 0;

    protected RenderPipeline(Shader shader)
    {
        Shader = shader;
    }

    public virtual bool BelongsToPipeline(Actor actor)
    {
        return false;
    }

    // Given the list of actors subscribed to the pipeline, generated the assets for later usage.
    public virtual void Setup(IEnumerable<Actor> actors, Director director)
    {
    }

    public virtual void Render(IEnumerable<Actor> actors, Director director)
    {
    }
}