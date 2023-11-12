namespace Kalasrapier.Engine.ImportJson
{
    public class ActorJson
    {
        public string id { get; set; }
        public string mesh_id { get; set; }
        public string? texture_id { get; set; }
        public bool enabled { get; set; }
        public float[] position { get; set; }
        public float[] scale { get; set; }
        public OrientationJson orientation { get; set; }

        // public ActorJson(float[] scale, string id, string sm, float[] position, OrientationJson orientation)
        // {
        //     this.scale = scale;
        //     this.id = id;
        //     this.mesh_id = sm;
        //     this.position = position;
        //     this.orientation = orientation;
        // }
    }
}

