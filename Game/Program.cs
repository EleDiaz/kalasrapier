using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

using Kalasrapier.Engine;
using Kalasrapier.Engine.Rendering;
using Kalasrapier.Engine.Rendering.Services;
using Kalasrapier.Game;

namespace Kalasrapier
{
    public class MyGameManager : GameManager
    {
        protected override void Start(World world)
        {
            world.LoadScene("Scenes/simple.json");
            world.AddRenderPipeline(new DefaultPipeline());
            Locator.ActorManager.ExtendActorBehavior<Pawn>("pawn_0");
        }

        
        // Main entry point.
        private static void Main()
        {
            var game = new MyGameManager();
            game.Run();
        }
    }
}
