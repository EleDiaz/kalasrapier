namespace Kalasrapier.Engine.ImportJson {
    public class MaterialFormat {
        public string name {get; set;}
        public float[] diffuse_color {get; set;}

        public MaterialFormat(string name, float[] diffuse_color)
        {
            this.name = name;
            this.diffuse_color = diffuse_color;
        }
    }
}

