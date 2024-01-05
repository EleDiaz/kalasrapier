namespace Kalasrapier.Engine.Rendering.Components;

public class CameraManager
{
    public Camera? ActiveCamera { get; set; }
    private List<Camera> Cameras { get; } = new();
    
    public void SetActiveCamera(Camera camera)
    {
        ActiveCamera = camera;
    }
    
    public void RegisterCamera(Camera camera)
    {
        Cameras.Add(camera);
        if (ActiveCamera is null)
        {
            ActiveCamera = camera;
        }
    }
    
    public void UnRegisterCamera(Camera camera)
    {
        Cameras.Remove(camera);
    }
}