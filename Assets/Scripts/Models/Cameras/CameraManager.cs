using Cinemachine;
using Maps.Classes;
using UnityEngine;

namespace Cameras
{
    public class CameraManager : MonoBehaviour
    {
        private CinemachineVirtualCamera _virtualCamera;
        private Minimap _minimap;

        public bool freeView = true;
    
        private Transform _selectedObjectTransform;
        private Transform _cameraSystemTransform;
        
        void Start()
        {
            //Find virtual camera
            _virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();   
            
            // Find camera system
            _cameraSystemTransform = FindObjectOfType<CameraSystem>().transform;
            
            // Initialize the first selected object as the camera system
            _selectedObjectTransform = _cameraSystemTransform;
            
            // Find minimap
            _minimap = FindObjectOfType<Minimap>();
            
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
                
                //Make the minimap the child of the camera system object
                _minimap.RepositionCamera(_cameraSystemTransform);

            }
            else
            {
                _virtualCamera.LookAt = _selectedObjectTransform;
                _virtualCamera.Follow = _selectedObjectTransform;   
                                
                //Make the minimap the child of the selected object
                _minimap.RepositionCamera(_selectedObjectTransform);
            }
            freeView = !freeView;
        }

        public void UpdateLastSelectedObject(Transform selectedObject)
        {
            _selectedObjectTransform = selectedObject; 
             //Make the minimap the child of the selected object
             _minimap.RepositionCamera(_selectedObjectTransform);
            freeView = true;
        }
    }
}
