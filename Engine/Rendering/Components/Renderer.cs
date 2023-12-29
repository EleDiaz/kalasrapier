namespace Kalasrapier.Engine.Rendering.Components;

public class Renderer: Component
{
    /// <summary>
    /// Bitmask of pipelines that this actor is subscribed to.
    /// </summary>
    public ulong RenderPipeline { get; set; }
    
}