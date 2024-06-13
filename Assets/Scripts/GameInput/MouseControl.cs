using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameInput
{
    public class MouseControl : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private SelectionManager selectionManager;

        private Vector3 lastPosition;

        private void Start()
        {
            DefaultOnClicked = selectionManager.OnSelectStart;
            DefaultOnReleased = selectionManager.OnSelectEnd;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (OnClicked != null)
                    OnClicked.Invoke();
                else
                    DefaultOnClicked?.Invoke();
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (OnReleased != null)
                    OnReleased.Invoke();
                else
                    DefaultOnReleased?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
                OnExit?.Invoke();
        }

        private event Action DefaultOnClicked, DefaultOnReleased;

        public event Action OnClicked, OnReleased, OnExit;


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
            if (Physics.Raycast(ray, out hit, 200f, groundLayer)) lastPosition = hit.point;

            return lastPosition;
        }
    }
}