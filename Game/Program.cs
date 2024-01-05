using Kalasrapier.Engine;
using Kalasrapier.Engine.Rendering;
using Kalasrapier.Engine.Rendering.Services;

namespace Kalasrapier.Game;

public class MyGameManager : GameManager
{
    protected override void Preload(Director director)
    {
        director.AddRenderPipeline(new DefaultPipeline());
        director.LoadScene("Scenes/simple.json");
        director.ActorManager.AddActor(new Pawn(director));
    }


    // Main entry point.
    private static void Main()
    {
        var game = new MyGameManager();
        game.Run();
    }
}