using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class ButtonActionManager : MonoBehaviour
{
    public string nextScene;
    public UIDocument mainUIDocument;
    public UIDocument settingsDocument;
    public UIDocument creditsDocument;

    private VisualElement settingsContainer;

    private void OnEnable()
    {
        var root = mainUIDocument.rootVisualElement;

        var startButton = root.Q<Button>("StartButton");
        var settingsButton = root.Q<Button>("SettingsButton");
        var creditButton = root.Q<Button>("CreditButton");
        var exitButton = root.Q<Button>("ExitButton");

        startButton?.RegisterCallback<ClickEvent>(ev => OnStartButtonClick());
        settingsButton?.RegisterCallback<ClickEvent>(ev => OnSettingsButtonClick());
        creditButton?.RegisterCallback<ClickEvent>(ev => OnCreditButtonClick());
        exitButton?.RegisterCallback<ClickEvent>(ev => OnExitButtonClick());

        settingsContainer = settingsDocument.rootVisualElement.Q<VisualElement>("Background");

        settingsDocument.rootVisualElement.style.display = DisplayStyle.None;
        creditsDocument.rootVisualElement.style.display = DisplayStyle.None;
    }

    private void OnStartButtonClick()
    {
        SceneManager.LoadScene(nextScene);
    }

    private void OnSettingsButtonClick()
    {
        settingsDocument.rootVisualElement.style.display = DisplayStyle.Flex;
        mainUIDocument.rootVisualElement.style.display = DisplayStyle.None; 
    }

    private void OnCreditButtonClick()
    {
        creditsDocument.rootVisualElement.style.display = DisplayStyle.Flex;
        mainUIDocument.rootVisualElement.style.display = DisplayStyle.None; 
    }

    private void OnExitButtonClick()
    {
        Application.Quit();
    }
}