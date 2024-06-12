using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class ButtonActionManager : MonoBehaviour
{
    public UIDocument mainUIDocument;
    public UIDocument settingsDocument;
    public UIDocument creditsDocument;

    private VisualElement settingsContainer;

    private void OnEnable()
    {
        // Get the root VisualElement
        var root = mainUIDocument.rootVisualElement;

        // Find buttons in the main UI document
        var startButton = root.Q<Button>("StartButton");
        var settingsButton = root.Q<Button>("SettingsButton");
        var creditButton = root.Q<Button>("CreditButton");
        var exitButton = root.Q<Button>("ExitButton");

        // Add event listeners to each button
        startButton?.RegisterCallback<ClickEvent>(ev => OnStartButtonClick());
        settingsButton?.RegisterCallback<ClickEvent>(ev => OnSettingsButtonClick());
        creditButton?.RegisterCallback<ClickEvent>(ev => OnCreditButtonClick());
        exitButton?.RegisterCallback<ClickEvent>(ev => OnExitButtonClick());

        settingsContainer = settingsDocument.rootVisualElement.Q<VisualElement>("Background");

        // Ensure the settings and credits documents are initially hidden
        settingsDocument.rootVisualElement.style.display = DisplayStyle.None;
        creditsDocument.rootVisualElement.style.display = DisplayStyle.None;
    }

    private void OnStartButtonClick()
    {
        SceneManager.LoadScene("MainScene"); // Replace "MainScene" with your actual scene name
    }

    private void OnSettingsButtonClick()
    {
        settingsDocument.rootVisualElement.style.display = DisplayStyle.Flex;
        mainUIDocument.rootVisualElement.style.display = DisplayStyle.None; // Hide the main menu
    }

    private void OnCreditButtonClick()
    {
        creditsDocument.rootVisualElement.style.display = DisplayStyle.Flex;
        mainUIDocument.rootVisualElement.style.display = DisplayStyle.None; // Hide the main menu
    }

    private void OnExitButtonClick()
    {
        Application.Quit();
    }
}