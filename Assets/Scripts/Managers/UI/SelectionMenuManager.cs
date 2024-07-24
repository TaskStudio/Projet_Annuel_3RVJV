using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SelectionMenuManager : MonoBehaviour
{
    public UIDocument mainUIDocument;
    private readonly string mainSceneName = "MainScene";
    private readonly string mapEditSceneName = "LoadScene";

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        var goBackButton = root.Q<Button>("GoBackButton");
        var soloModeElement = root.Q<VisualElement>("SoloMode");
        var editMapModeElement = root.Q<VisualElement>("EditMapMode");

        if (goBackButton != null)
            goBackButton.RegisterCallback<ClickEvent>(ev => GoBackToMainMenu());
        else
            Debug.LogError("GoBackButton not found in the UXML document");

        if (soloModeElement != null)
            soloModeElement.RegisterCallback<ClickEvent>(ev => LoadScene(mainSceneName));
        else
            Debug.LogError("SoloMode element not found in the UXML document");

        if (editMapModeElement != null)
            editMapModeElement.RegisterCallback<ClickEvent>(ev => LoadScene(mapEditSceneName));
        else
            Debug.LogError("Edit map mode element not found in the UXML document");
    }

    private void GoBackToMainMenu()
    {
        // Hide selection menu UI
        var root = GetComponent<UIDocument>().rootVisualElement;
        root.style.display = DisplayStyle.None;

        // Show main menu UI
        mainUIDocument.rootVisualElement.style.display = DisplayStyle.Flex;
    }

    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}