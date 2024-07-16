using UnityEngine;
using UnityEngine.UIElements;

public class SelectionMenuManager : MonoBehaviour
{
    public UIDocument mainUIDocument;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        var goBackButton = root.Q<Button>("GoBackButton");

        if (goBackButton != null)
        {
            goBackButton.RegisterCallback<ClickEvent>(ev => GoBackToMainMenu());
        }
        else
        {
            Debug.LogError("GoBackButton not found in the UXML document");
        }
    }

    private void GoBackToMainMenu()
    {
        // Hide credits UI
        var root = GetComponent<UIDocument>().rootVisualElement;
        root.style.display = DisplayStyle.None;

        // Show main menu UI
        mainUIDocument.rootVisualElement.style.display = DisplayStyle.Flex;
    }
}