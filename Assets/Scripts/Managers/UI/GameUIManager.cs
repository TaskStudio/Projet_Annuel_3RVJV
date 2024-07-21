using UnityEngine;
using UnityEngine.UIElements;

public class GameUIManager : MonoBehaviour
{
    public UIDocument winUIDocument;
    public UIDocument loseUIDocument;
    public UnityEngine.UI.RawImage minimapRawImage; 

    private VisualElement winRoot;
    private VisualElement loseRoot;

    private void Start()
    {
        if (winUIDocument != null)
        {
            winRoot = winUIDocument.rootVisualElement;
            winRoot.style.display = DisplayStyle.None;

            var winRestartButton = winRoot.Q<Button>("Restart");
            var winExitButton = winRoot.Q<Button>("Exit");

            winRestartButton.clicked += RestartGame;
            winExitButton.clicked += ExitGame;
        }

        if (loseUIDocument != null)
        {
            loseRoot = loseUIDocument.rootVisualElement;
            loseRoot.style.display = DisplayStyle.None;

            var loseRestartButton = loseRoot.Q<Button>("Restart");
            var loseExitButton = loseRoot.Q<Button>("Exit");

            loseRestartButton.clicked += RestartGame;
            loseExitButton.clicked += ExitGame;
        }
    }

    public void ShowWinScreen()
    {
        if (winRoot != null)
        {
            winRoot.style.display = DisplayStyle.Flex;
            UpdateStatistics(winRoot);
            StatManager.Instance.StopTimer();
            DisableMinimap(); // Disable minimap when game ends
        }
    }

    public void ShowLoseScreen()
    {
        if (loseRoot != null)
        {
            loseRoot.style.display = DisplayStyle.Flex;
            UpdateStatistics(loseRoot);
            StatManager.Instance.StopTimer();
            DisableMinimap(); // Disable minimap when game ends
        }
    }

    private void UpdateStatistics(VisualElement root)
    {
        if (root == null)
        {
            Debug.LogError("Root VisualElement is null.");
            return;
        }

        SetLabelText(root, "TimeElapsed", StatManager.Instance.GetFormattedElapsedTime());
        SetLabelText(root, "EntitiesKilled", StatManager.EnemyDeathCount.ToString());
        SetLabelText(root, "EntitiesLost", StatManager.AllyDeathCount.ToString());
        SetLabelText(root, "EntitiesCreated", StatManager.GetUnitProductionCount().ToString());
        SetLabelText(root, "DamageDealt", StatManager.EnemyDamageTaken.ToString());
        SetLabelText(root, "DamageReceived", StatManager.AllyDamageTaken.ToString());
        SetLabelText(root, "ResourcesCollected",
            (StatManager.GetAllyWoodCollected() + StatManager.GetAllyStoneCollected() +
             StatManager.GetAllyGoldCollected()).ToString());
        SetLabelText(root, "WoodCollected", StatManager.GetAllyWoodCollected().ToString());
        SetLabelText(root, "StoneCollected", StatManager.GetAllyStoneCollected().ToString());
        SetLabelText(root, "GoldCollected", StatManager.GetAllyGoldCollected().ToString());
        SetLabelText(root, "ResourcesUsed",
            (StatManager.GetAllyWoodSpent() + StatManager.GetAllyStoneSpent() + StatManager.GetAllyGoldSpent())
            .ToString());
        SetLabelText(root, "WoodUsed", StatManager.GetAllyWoodSpent().ToString());
        SetLabelText(root, "StoneUsed", StatManager.GetAllyStoneSpent().ToString());
        SetLabelText(root, "GoldUsed", StatManager.GetAllyGoldSpent().ToString());
    }

    private void SetLabelText(VisualElement root, string labelName, string text)
    {
        var label = root.Q<Label>(labelName);
        if (label != null)
            label.text = text;
        else
            Debug.LogError($"Label with name '{labelName}' not found in the UXML.");
    }

    private void RestartGame()
    {
        GameManager.Instance.RestartGame();
    }

    private void ExitGame()
    {
        GameManager.Instance.ExitGame();
    }

    private void DisableMinimap()
    {
        if (minimapRawImage != null)
        {
            minimapRawImage.enabled = false;
        }
    }
}
