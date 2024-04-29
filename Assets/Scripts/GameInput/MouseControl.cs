using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameInput
{
    public class MouseControl : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private LayerMask groundLayer;

        private Vector3 lastPosition;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
                OnClicked?.Invoke();
            if (Input.GetKeyDown(KeyCode.Escape))
                OnExit?.Invoke();
        }

        public event Action OnClicked, OnExit;


        public bool IsPointerOverUI()
        {
            return EventSystem.current.IsPointerOverGameObject();
        }


        public Vector3 GetCursorMapPosition()
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = mainCamera.nearClipPlane;
            Ray ray = mainCamera.ScreenPointToRay(mousePos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, groundLayer)) lastPosition = hit.point;

            return lastPosition;
        }
    }
}