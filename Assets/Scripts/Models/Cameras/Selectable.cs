using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public class Selectable : MonoBehaviour, IPointerClickHandler
{
    private Transform _selectedObjectTransform;
    private CinemachineVirtualCamera _virtualCamera;

    private void Awake()
    {
        _selectedObjectTransform = transform;
    }

    private void Start()
    {
        _virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _virtualCamera.LookAt = _selectedObjectTransform;
        _virtualCamera.Follow = _selectedObjectTransform;

        CameraManager cameraManager = FindObjectOfType<CameraManager>();
        cameraManager.UpdateLastSelectedObject(_selectedObjectTransform);
    }
}