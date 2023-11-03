namespace Kalasrapier.Engine.ImportJson
{
    public class Orientation
    {
        public float[] axis { get; set; }
        public float angle { get; set; }

        public Orientation()
        {
            this.axis = new float[] { 0.0f, 1.0f, 0.0f };
            this.angle = 0f;
        }

        public Orientation(float[] axis, float angle)
        {
            this.axis = axis;
            this.angle = angle;
        }
    }

    public class Actor
    {
        public string id { get; set; }
        public string sm { get; set; }
        public bool enabled { get; set; }
        public float[] position { get; set; }
        public float[] scale { get; set; }
        public Orientation orientation { get; set; }

        public Actor(float[] scale, string id, string sm, float[] position, Orientation orientation)
        {
            this.scale = scale;
            this.id = id;
            this.sm = sm;
            this.position = position;
            this.orientation = orientation;
        }
    }

    public class MeshMeta
    {
        public string file { get; set; }
        public string id { get; set; }

        public MeshMeta(string file, string id)
        {
            this.file = file;
            this.id = id;
        }
    }

    public class Scene
    {
        public MeshMeta[] meshes { get; set; }
        public Actor[] actors { get; set; }
        public PlayerStart playerStart { get; set; }

        public Scene(MeshMeta[] meshes, Actor[] actors, PlayerStart playerStart)
        {
            this.meshes = meshes;
            this.actors = actors;
            this.playerStart = playerStart;
        }
    }

    public class PlayerStart
    {
        public float[] position { get; set; }
        public Orientation orientation { get; set; }

        public PlayerStart()
        {
            this.position = new float[] { 1.0f, 1.0f, 1.0f };
            this.orientation = new Orientation();
        }

        public PlayerStart(Orientation orientation, float[] position)
        {
            this.orientation = orientation;
            this.position = position;
        }
    }
}
