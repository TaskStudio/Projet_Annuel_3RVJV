using UnityEngine;

namespace GameInput
{
    public class KeyboardControl : MonoBehaviour
    {
        [SerializeField] private SelectionManager selectionManager;
        private GameplayInput gameplayInput;

        private void Awake()
        {
            gameplayInput = new GameplayInput();

            gameplayInput.Actionable.Action1.performed += _ => selectionManager.OnInvokeActionable(0);
            gameplayInput.Actionable.Action2.performed += _ => selectionManager.OnInvokeActionable(1);
            gameplayInput.Actionable.Action3.performed += _ => selectionManager.OnInvokeActionable(2);
            gameplayInput.Actionable.Action4.performed += _ => selectionManager.OnInvokeActionable(3);
        }

        private void OnEnable()
        {
            gameplayInput.Enable();
        }

        private void OnDisable()
        {
            gameplayInput.Disable();
        }
    }
}