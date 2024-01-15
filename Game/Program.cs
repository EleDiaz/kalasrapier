using Kalasrapier.Engine;
using Kalasrapier.Engine.Rendering.Services;

namespace Kalasrapier.Game;

public class MyGameManager : GameManager
{
    protected override void Preload(Director director)
    {
        director.AddRenderPipeline(new DefaultPipeline());
        director.AddRenderPipeline(new TimerPipeline());
        director.LoadScene("Scenes/simple.json");
        director.ActorManager.AddActor(new Pawn(director));
        director.ActorManager.AddActor(new TimerPawn(director));
    }


    // Main entry point.
    private static void Main()
    {
        var game = new MyGameManager();
        game.Run();
    }
}