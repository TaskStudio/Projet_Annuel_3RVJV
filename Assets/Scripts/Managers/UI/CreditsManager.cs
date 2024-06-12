using UnityEngine;
using UnityEngine.UIElements;

public class CreditsManager : MonoBehaviour
{
    public UIDocument mainUIDocument;

    private void OnEnable()
    {
        Debug.Log("CreditsManager OnEnable");

        var root = GetComponent<UIDocument>().rootVisualElement;

        var goBackButton = root.Q<Button>("GoBackButton");

        if (goBackButton != null)
        {
            Debug.Log("GoBackButton found, registering callback");
            goBackButton.RegisterCallback<ClickEvent>(ev => GoBackToMainMenu());
        }
        else
        {
            Debug.LogError("GoBackButton not found in the UXML document");
        }
    }

    private void GoBackToMainMenu()
    {
        Debug.Log("GoBackButton CLICKED!");

        // Hide credits UI
        var root = GetComponent<UIDocument>().rootVisualElement;
        root.style.display = DisplayStyle.None;

        // Show main menu UI
        mainUIDocument.rootVisualElement.style.display = DisplayStyle.Flex;
    }
}