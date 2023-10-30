using System.Text.Json;

namespace Kalasrapier {
    public class Level {
        private string? _meshFilePath;

        private List<RetrievedMesh>? _levelMeshes = new List<RetrievedMesh>();
        private readonly JsonSerializerOptions _jsonOptions= new();

        public Level()
        {

        }

        public Level(string pathMeshFile) {
            _meshFilePath = pathMeshFile;
        }


        public void LoadLevel() {
            string ?text = null;
            if (_meshFilePath is not null) {
                text = File.ReadAllText(_meshFilePath);
            }
            if (text is not null && _jsonOptions is not null) {
                _levelMeshes = JsonSerializer.Deserialize<List<RetrievedMesh>>(text, _jsonOptions); 
            }
        }

        public void GetFirstMesh(out float[] ?vertexData, out int [] ?indexData){
            vertexData = null;
            indexData = null;
            
            if(_levelMeshes is null){
            return;
            }

            var mesh = _levelMeshes.First();
            int nvertices = mesh.vertexdata.Length;
            int ncolors = mesh.colordata.Length;
            if(nvertices/3 != ncolors/4) {
                return;
            }

            vertexData = new float[nvertices + ncolors];

            for(int i=0,j=0,k=0;i<mesh.vertexdata.Length;i=i+3,j=j+4,k=k+7) {
                vertexData[k]=mesh.vertexdata[i];
                vertexData[k+1]=mesh.vertexdata[i+1];
                vertexData[k+2]=mesh.vertexdata[i+2];
                vertexData[k+3]=mesh.colordata[j];
                vertexData[k+4]=mesh.colordata[j+1];
                vertexData[k+5]=mesh.colordata[j+2];
                vertexData[k+6]=mesh.colordata[j+3];
            }
            foreach(var v in vertexData)
                Console.WriteLine(v);

            indexData = mesh.indexdata;
        }
    }
}