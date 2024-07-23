using System;
using UnityEngine;

namespace GameInput
{
    public class MouseControl : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private SelectionManager selectionManager;
        public bool isMapEditing;

        private Vector3 lastPosition;

        private void Start()
        {
            DefaultOnClicked = selectionManager.OnSelectStart;
            DefaultOnReleased = selectionManager.OnSelectEnd;
            if (!isMapEditing)
                DefaultOnRightClicked = UnitsManager.Instance.SetUnitsOrder;
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

            if (Input.GetMouseButtonDown(1))
            {
                if (OnRightClicked != null)
                    OnRightClicked.Invoke();
                else
                    DefaultOnRightClicked?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
                OnExit?.Invoke();
        }

        private event Action DefaultOnClicked, DefaultOnReleased, DefaultOnRightClicked;

        public event Action OnClicked, OnReleased, OnRightClicked, OnExit;

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