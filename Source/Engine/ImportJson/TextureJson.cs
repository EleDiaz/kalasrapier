namespace Kalasrapier.Engine.ImportJson
{
    public class TextureJson
    {
        public string path { get; set; }
        public string id { get; set; }

        public TextureJson(string path, string id)
        {
            this.path = path;
            this.id = id;
        }
    }
}
