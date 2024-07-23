using UnityEngine;

public class Minimap : MonoBehaviour
{
    private Transform _cameraSystemTransform;
    
    private void Start()
    {
        // Find camera system
        _cameraSystemTransform = FindObjectOfType<CameraSystem>().transform;
        transform.SetParent(_cameraSystemTransform, false);
    }

    public void RepositionCamera(Transform newCameraPosition)
    {
        var minimapTransform = transform;
        minimapTransform.SetParent(newCameraPosition, false);
        minimapTransform.position = newCameraPosition.position;
    }
}