using Kalasrapier.Engine;
using Kalasrapier.Engine.Rendering;
using Kalasrapier.Engine.Rendering.Services;

namespace Kalasrapier.Game;

public class MyGameManager : GameManager
{
    protected override void Preload(World world)
    {
        world.AddRenderPipeline(new DefaultPipeline());
        world.LoadScene("Scenes/simple.json");
        Locator.ActorManager.ExtendActorBehavior<Pawn>("pawn_0");
    }


    // Main entry point.
    private static void Main()
    {
        var game = new MyGameManager();
        game.Run();
    }
}