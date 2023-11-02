namespace Kalasrapier
{
    public class Orientation
    {
        public float[] axis { get; set; }
        public float angle { get; set; }
    }

    public class Actor
    {
        public string id { get; set; }
        public string sm { get; set; }
        public bool enabled { get; set; }
        public float[] position { get; set; }
        public float[] scale { get; set; }
        public Orientation orientation { get; set; }
    }

    public class MeshMeta
    {
        public string file { get; set; }
        public string id { get; set; }


    }
    public class Scene
    {
        public MeshMeta[] meshes { get; set; }

        public Actor[] actors { get; set; }

        public PlayerStart playerStart { get; set;}
    }

    public class PlayerStart {
        public float[] position { get; set; }
        public Orientation orientation { get; set; }
    }
}
