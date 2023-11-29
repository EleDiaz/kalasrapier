namespace Kalasrapier.Engine.Rendering
{
    public class ActorManager
    {
        private Dictionary<string, Actor> Actors { get; set; } = new();
        
        public void AddActor(Actor actor)
        {
            Actors.Add(actor.Id, actor);
        }
    }
}