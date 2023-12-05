using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

using Kalasrapier.Engine;
using Kalasrapier.Game;

namespace Kalasrapier
{
    public static class Program
    {
        private static void Main()
        {
            var game = new GameManager();
            game.World.LoadScene("Scenes/simple.json");
            game.World.AddRenderPipeline(new DefaultPipeline());
            game.World.ActorManager.ExtendActorBehavior<Pawn>("pawn_0");
            
            game.Run();
        }
    }
}
