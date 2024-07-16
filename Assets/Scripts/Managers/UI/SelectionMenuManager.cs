using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SelectionMenuManager : MonoBehaviour
{
    public UIDocument mainUIDocument;
    public string mainSceneName = "MainScene";

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        var goBackButton = root.Q<Button>("GoBackButton");
        var soloModeElement = root.Q<VisualElement>("SoloMode");

        if (goBackButton != null)
        {
            goBackButton.RegisterCallback<ClickEvent>(ev => GoBackToMainMenu());
        }
        else
        {
            Debug.LogError("GoBackButton not found in the UXML document");
        }

        if (soloModeElement != null)
        {
            soloModeElement.RegisterCallback<ClickEvent>(ev => LoadMainScene());
        }
        else
        {
            Debug.LogError("SoloMode element not found in the UXML document");
        }
    }

    private void GoBackToMainMenu()
    {
        // Hide selection menu UI
        var root = GetComponent<UIDocument>().rootVisualElement;
        root.style.display = DisplayStyle.None;

        // Show main menu UI
        mainUIDocument.rootVisualElement.style.display = DisplayStyle.Flex;
    }

    private void LoadMainScene()
    {
        // Load the main scene
        SceneManager.LoadScene(mainSceneName);
    }
}