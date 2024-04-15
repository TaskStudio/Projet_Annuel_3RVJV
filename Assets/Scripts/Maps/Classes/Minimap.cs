using Cameras;
using UnityEngine;

namespace Maps.Classes
{
    public class Minimap : MonoBehaviour
    {
        
        private Transform _cameraSystemTransform;
        void Start()
        {
            // Find camera system
            _cameraSystemTransform = FindObjectOfType<CameraSystem>().transform;
            this.transform.SetParent(_cameraSystemTransform, false);
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void RepositionCamera(Transform newCameraPosition)
        {

            Transform minimapTransform = this.transform;
            minimapTransform.SetParent(newCameraPosition, false);
            minimapTransform.position = newCameraPosition.position;
        }
    }
}
