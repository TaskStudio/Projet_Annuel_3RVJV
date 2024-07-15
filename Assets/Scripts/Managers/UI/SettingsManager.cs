using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class SettingsManager : MonoBehaviour
{
    private VisualElement generalPanel;
    private VisualElement graphicsPanel;
    private VisualElement audioPanel;
    private VisualElement languagePanel;
    private Button currentButton;
    private DropdownField fullscreenDropdown;
    private DropdownField resolutionDropdown;

    public UIDocument mainUIDocument;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        var generalButton = root.Q<Button>("GeneralButton");
        var graphicsButton = root.Q<Button>("GraphicsButton");
        var audioButton = root.Q<Button>("AudioButton");
        var languageButton = root.Q<Button>("LanguageButton");
        var aboutButton = root.Q<Button>("AboutButton");
        var goBackButton = root.Q<Button>("GoBackButton");

        generalPanel = root.Q<VisualElement>("GeneralPanel");
        graphicsPanel = root.Q<VisualElement>("GraphicsPanel");
        audioPanel = root.Q<VisualElement>("AudioPanel");
        languagePanel = root.Q<VisualElement>("LanguagePanel");

        generalButton?.RegisterCallback<ClickEvent>(ev => ShowPanel(generalPanel, generalButton));
        graphicsButton?.RegisterCallback<ClickEvent>(ev => ShowPanel(graphicsPanel, graphicsButton));
        audioButton?.RegisterCallback<ClickEvent>(ev => ShowPanel(audioPanel, audioButton));
        languageButton?.RegisterCallback<ClickEvent>(ev => ShowPanel(languagePanel, languageButton));
        goBackButton?.RegisterCallback<ClickEvent>(ev => GoBackToMainMenu());

        // Show the General panel by default
        ShowPanel(generalPanel, generalButton);

        // Initialize and register callback for the fullscreen dropdown
        fullscreenDropdown = root.Q<DropdownField>("WindowOrScreened");
        if (fullscreenDropdown != null)
        {
            fullscreenDropdown.choices = new List<string> { "Windowed", "Full Screen" };
            fullscreenDropdown.RegisterValueChangedCallback(evt => SetFullScreenMode(evt.newValue));
            InitializeScreenModeDropdown();
        }

        // Initialize and register callback for the resolution dropdown
        resolutionDropdown = root.Q<DropdownField>("Resolution");
        if (resolutionDropdown != null)
        {
            resolutionDropdown.RegisterValueChangedCallback(evt => SetResolution(evt.newValue));
        }
    }

    private void ShowPanel(VisualElement panelToShow, Button button)
    {
        // Hide all panels
        generalPanel.style.display = DisplayStyle.None;
        graphicsPanel.style.display = DisplayStyle.None;
        audioPanel.style.display = DisplayStyle.None;
        languagePanel.style.display = DisplayStyle.None;

        // Show the selected panel
        panelToShow.style.display = DisplayStyle.Flex;

        // Update button classes
        if (currentButton != null)
        {
            currentButton.RemoveFromClassList("currentPanel");
        }

        currentButton = button;
        currentButton.AddToClassList("currentPanel");
    }

    private void GoBackToMainMenu()
    {
        // Hide settings UI
        var root = GetComponent<UIDocument>().rootVisualElement;
        root.style.display = DisplayStyle.None;

        // Show main menu UI
        mainUIDocument.rootVisualElement.style.display = DisplayStyle.Flex;
    }

    private void SetFullScreenMode(string mode)
    {
        if (mode == "Windowed")
        {
            int newWidth = (int)(Screen.currentResolution.width * 0.7f);
            int newHeight = (int)(Screen.currentResolution.height * 0.7f);
            Screen.SetResolution(newWidth, newHeight, FullScreenMode.Windowed);
        }
        else if (mode == "Full Screen")
        {
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.ExclusiveFullScreen);
        }

        Debug.Log("Screen mode set to: " + mode);
    }

    private void SetResolution(string resolution)
    {
        var resolutionParts = resolution.Split('x');
        if (resolutionParts.Length == 2)
        {
            int width = int.Parse(resolutionParts[0]);
            int height = int.Parse(resolutionParts[1]);
            Screen.SetResolution(width, height, Screen.fullScreenMode);
            Debug.Log($"Resolution set to: {width}x{height}");
        }
    }

    private void InitializeScreenModeDropdown()
    {
        // Set the dropdown to the current screen mode
        if (Screen.fullScreenMode == FullScreenMode.Windowed)
        {
            fullscreenDropdown.SetValueWithoutNotify("Windowed");
        }
        else if (Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen || Screen.fullScreenMode == FullScreenMode.FullScreenWindow)
        {
            fullscreenDropdown.SetValueWithoutNotify("Full Screen");
        }
    }
}