using Cinemachine;
using Maps.Classes;
using UnityEngine;

namespace Cameras
{
    public class CameraSystem : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera virtualCamera; 
    
        [SerializeField] private float fovMax = 40; 
        [SerializeField] private float fovMin = 10; 
        private float _targetFov = 40;
        [SerializeField] private float zoomSpeed = 10f; 

        [SerializeField] private bool useEdgeScrolling = true;
        [SerializeField] private float cameraMoveSpeed = 50f;

        [SerializeField] private float cameraRotateDir;
        [SerializeField] private float cameraRotateSpeed = 100f;

        [SerializeField] private int edgeScrollSize = 30;
    
        private Transform _cachedTransform;
    
        private Map _gameMap;

        private void Start()
        {
            // Cache the transform at the start
            _cachedTransform = transform;
        
            // Find the Map component in the scene
            _gameMap = FindObjectOfType<Map>();
        }

        // Update is called once per frame
        void Update()
        {
            //Edges movements mechanisms
            if (useEdgeScrolling)
            {
                HandleEdgesMovement();
            }
            //Rotation mechanisms
            HandleRotation();
            HandleZoom();
        }

        private void HandleEdgesMovement()
        {
            //Movement variables
            var inputDir = new Vector3(0, 0, 0);
        
            if (Input.mousePosition.x < edgeScrollSize) inputDir.x = -1f;
            if (Input.mousePosition.y < edgeScrollSize) inputDir.z = -1f;
            if (Input.mousePosition.x > Screen.width - edgeScrollSize) inputDir.x = +1f;
            if (Input.mousePosition.y > Screen.height - edgeScrollSize) inputDir.z = +1f;
    
            //Movements mechanisms
            var cameraMoveDir = _cachedTransform.forward * inputDir.z + _cachedTransform.right * inputDir.x;
            var proposedPosition = _cachedTransform.position + cameraMoveDir * (cameraMoveSpeed * Time.deltaTime);
        
            var clampedPosition = _gameMap.ClampPositionToLimits(proposedPosition);
            _cachedTransform.position = clampedPosition;
        }
        private void HandleRotation()
        {
            //Rotations mechanisms
            cameraRotateDir = 0f;
            if (Input.GetKey(KeyCode.A)) cameraRotateDir = +1f;
            if (Input.GetKey(KeyCode.E)) cameraRotateDir = -1f;
        
            transform.eulerAngles += new Vector3(0, cameraRotateDir * cameraRotateSpeed * Time.deltaTime, 0);
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
}
