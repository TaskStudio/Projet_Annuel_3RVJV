using UnityEngine;
using UnityEngine.UIElements;

public class SettingsManager : MonoBehaviour
{
    private VisualElement generalPanel;
    private VisualElement graphicsPanel;
    private VisualElement audioPanel;
    private VisualElement languagePanel;
    private Button currentButton;

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
}