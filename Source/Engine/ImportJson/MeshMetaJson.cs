namespace Kalasrapier.Engine.ImportJson
{
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
}

