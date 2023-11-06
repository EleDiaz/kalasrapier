namespace Kalasrapier.Engine.ImportJson
{
    public class OrientationJson
    {
        public float[] axis { get; set; }
        public float angle { get; set; }

        public OrientationJson()
        {
            this.axis = new float[] { 0.0f, 1.0f, 0.0f };
            this.angle = 0f;
        }

        public OrientationJson(float[] axis, float angle)
        {
            this.axis = axis;
            this.angle = angle;
        }
    }

    public class ActorJson
    {
        public string id { get; set; }
        public string sm { get; set; }
        public bool enabled { get; set; }
        public float[] position { get; set; }
        public float[] scale { get; set; }
        public OrientationJson orientation { get; set; }

        public ActorJson(float[] scale, string id, string sm, float[] position, OrientationJson orientation)
        {
            this.scale = scale;
            this.id = id;
            this.sm = sm;
            this.position = position;
            this.orientation = orientation;
        }
    }

    public class MeshMetaJson
    {
        public string file { get; set; }
        public string id { get; set; }

        public MeshMetaJson(string file, string id)
        {
            this.file = file;
            this.id = id;
        }
    }

    public class SceneJson
    {
        public MeshMetaJson[] meshes { get; set; }
        public ActorJson[] actors { get; set; }
        public PlayerStartJson playerStart { get; set; }

        public SceneJson(MeshMetaJson[] meshes, ActorJson[] actors, PlayerStartJson playerStart)
        {
            this.meshes = meshes;
            this.actors = actors;
            this.playerStart = playerStart;
        }
    }

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
