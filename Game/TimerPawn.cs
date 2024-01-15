using Kalasrapier.Engine.Rendering.Actors;
using Kalasrapier.Engine.Rendering.Services;

namespace Kalasrapier.Game;

public class TimerPawn : Actor
{
    public float Time { get; set; } = 20;
    public int TotalTime { get; set; } = 20;
    public float Percentage => (float)Time / TotalTime;
    
    public TimerPawn(Director director) : base(director)
    {
        Tag = "Timer";
    }


    public override void Update(double deltaTime)
    {
        Time -= (float)deltaTime;
        
        Console.WriteLine(Time);
        if (Time <= 0)
        {
            Time = 0;
        }
    }
}