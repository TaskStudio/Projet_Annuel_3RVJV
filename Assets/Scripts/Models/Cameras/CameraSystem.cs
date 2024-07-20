using Cinemachine;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    [SerializeField] private float fovMax = 40;
    [SerializeField] private float fovMin = 10;
    [SerializeField] private float zoomSpeed = 10f;

    [SerializeField] private bool useEdgeScrolling = true;
    [SerializeField] private float cameraMoveSpeed = 50f;

    [SerializeField] private float cameraRotateSpeed = 100f;

    [SerializeField] private int edgeScrollSize = 30;

    private Transform _cachedTransform;

    private Map _gameMap;
    private float _targetFov = 40;

    private void Start()
    {
        // Cache the transform at the start
        _cachedTransform = transform;

        // Find the Map component in the scene
        _gameMap = FindObjectOfType<Map>();
    }

    // Update is called once per frame
    private void Update()
    {
        // Movement mechanisms
        HandleMovement();
        // Rotation mechanisms
        HandleRotation();
        // Zoom mechanisms
        HandleZoom();
    }

    private void HandleMovement()
    {
        // Movement variables
        var inputDir = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.Q)) inputDir.x = -1f;
        if (Input.GetKey(KeyCode.D)) inputDir.x = +1f;
        if (Input.GetKey(KeyCode.Z)) inputDir.z = +1f;
        if (Input.GetKey(KeyCode.S)) inputDir.z = -1f;

        // Movements mechanisms
        var cameraMoveDir = _cachedTransform.forward * inputDir.z + _cachedTransform.right * inputDir.x;
        var proposedPosition = _cachedTransform.position + cameraMoveDir * (cameraMoveSpeed * Time.deltaTime);

        var clampedPosition = _gameMap.ClampPositionToLimits(proposedPosition);
        _cachedTransform.position = clampedPosition;
    }

    private void HandleRotation()
    {
        // Check if the middle mouse button is held down
        if (Input.GetMouseButton(2))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            // Apply rotation around the Y-axis (horizontal rotation)
            _cachedTransform.eulerAngles += new Vector3(0, mouseX * cameraRotateSpeed * Time.deltaTime, 0);

            // Optionally, you can add vertical rotation by uncommenting the line below
            // _cachedTransform.eulerAngles += new Vector3(-mouseY * cameraRotateSpeed * Time.deltaTime, 0, 0);
        }
    }

    private void HandleZoom()
    {
        if (Input.mouseScrollDelta.y < 0) _targetFov += 5;
        if (Input.mouseScrollDelta.y > 0) _targetFov -= 5;
        _targetFov = Mathf.Clamp(_targetFov, fovMin, fovMax);

        virtualCamera.m_Lens.FieldOfView =
            Mathf.Lerp(virtualCamera.m_Lens.FieldOfView, _targetFov, Time.deltaTime * zoomSpeed);
    }
}