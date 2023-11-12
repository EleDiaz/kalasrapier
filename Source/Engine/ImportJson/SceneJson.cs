namespace Kalasrapier.Engine.ImportJson
{
    public class SceneJson
    {
        public MeshMetaJson[] meshes { get; set; }
        public TextureJson[] textures { get; set; }
        public ActorJson[] actors { get; set; }
        public PlayerStartJson playerStart { get; set; }

        public SceneJson(MeshMetaJson[] meshes, ActorJson[] actors, PlayerStartJson playerStart, TextureJson[] textures)
        {
            this.meshes = meshes;
            this.actors = actors;
            this.playerStart = playerStart;
            this.textures = textures;
        }
    }
}
