
namespace Kalasrapier {

    public class RetrievedMesh {
        public int nvertex;

        public float[] vertexdata {get; set;}
        public float[] colordata {get; set;}
        public int nindex;

        public int[] indexdata {get; set;}

        public RetrievedMesh() {
            vertexdata = new float[0];
            colordata = new float[0];
            indexdata = new int[0];
        }
    }
}