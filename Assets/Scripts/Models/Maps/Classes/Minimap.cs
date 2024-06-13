using UnityEngine;

public class Minimap : MonoBehaviour
{
    [SerializeField] private Camera miniMapCamera;

    [SerializeField] private float fovMax = 40;
    [SerializeField] private float fovMin = 10;
    [SerializeField] private float zoomSpeed = 1f;

    private Transform _cameraSystemTransform;
    private float _targetFov = 40;

    private void Start()
    {
        // Find camera system
        _cameraSystemTransform = FindObjectOfType<CameraSystem>().transform;
        transform.SetParent(_cameraSystemTransform, false);
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void RepositionCamera(Transform newCameraPosition)
    {
        Transform minimapTransform = transform;
        minimapTransform.SetParent(newCameraPosition, false);
        minimapTransform.position = newCameraPosition.position;
    }

    private void HandleZoom()
    {
        if (Input.mouseScrollDelta.y < 0) _targetFov += 1;
        if (Input.mouseScrollDelta.y > 0) _targetFov -= 1;
        _targetFov = Mathf.Clamp(_targetFov, fovMin, fovMax);

        miniMapCamera.orthographicSize =
            Mathf.Lerp(miniMapCamera.orthographicSize, _targetFov, Time.deltaTime * zoomSpeed);
    }
}