namespace Kalasrapier.Engine.ImportJson
{
    public class PlayerStartJson
    {
        public float[] position { get; set; }
        public OrientationJson orientation { get; set; }

        public PlayerStartJson()
        {
            this.position = new float[] { 1.0f, 1.0f, 1.0f };
            this.orientation = new OrientationJson();
        }

        public PlayerStartJson(OrientationJson orientation, float[] position)
        {
            this.orientation = orientation;
            this.position = position;
        }
    }
}

