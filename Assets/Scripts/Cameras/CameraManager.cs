using Cinemachine;
using UnityEngine;

namespace Cameras
{
    public class CameraManager : MonoBehaviour
    {
        private CinemachineVirtualCamera _virtualCamera;
    
        public bool freeView = true;
    
        private Transform _selectedObjectTransform;
        private Transform _cameraSystemTransform;
    
        // Start is called before the first frame update
        void Start()
        {
            _virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();    
            _cameraSystemTransform = FindObjectOfType<CameraSystem>().transform;
            _selectedObjectTransform = _cameraSystemTransform;
            UpdateView();
        }

        // Update is called once per frame
        void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Y)) return;
            UpdateView();
        }

        void UpdateView()
        {
            if (freeView)
            {
                var position = _selectedObjectTransform.position;
                var rotation = _selectedObjectTransform.rotation;
                _cameraSystemTransform.position = new Vector3(position.x, position.y, position.z); // Camera reposition to last selected Object
                _cameraSystemTransform.rotation = new Quaternion(rotation.x, rotation.y, rotation.z,0); // Camera reposition to last selected Object
            
                _virtualCamera.LookAt = _cameraSystemTransform;
                _virtualCamera.Follow = _cameraSystemTransform;
            }
            else
            {
                _virtualCamera.LookAt = _selectedObjectTransform;
                _virtualCamera.Follow = _selectedObjectTransform;        
            }
            freeView = !freeView;
        }

        public void UpdateLastSelectedObject(Transform selectedObject)
        {
            _selectedObjectTransform = selectedObject;
            freeView = true;
        }
    }
}
