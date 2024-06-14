using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public string nextScene;
    public UIDocument winUIDocument;
    public UIDocument loseUIDocument;
    private VisualElement loseRoot;

    private VisualElement winRoot;

    private void Start()
    {
        winRoot = winUIDocument.rootVisualElement;
        loseRoot = loseUIDocument.rootVisualElement;

        winRoot.style.display = DisplayStyle.None;
        loseRoot.style.display = DisplayStyle.None;

        var winRestartButton = winRoot.Q<Button>("RestartButton");
        var winExitButton = winRoot.Q<Button>("ExitButton");

        var loseRestartButton = loseRoot.Q<Button>("RestartButton");
        var loseExitButton = loseRoot.Q<Button>("ExitButton");

        winRestartButton.clicked += RestartGame;
        winExitButton.clicked += ExitGame;

        loseRestartButton.clicked += RestartGame;
        loseExitButton.clicked += ExitGame;
    }

    private void Update()
    {
        if (CheckWinCondition())
            ShowWinScreen();
        else if (CheckLoseCondition()) ShowLoseScreen();
    }

    private bool CheckWinCondition()
    {
        // Sebi met la condition win ici stp
        return false;
    }

    private bool CheckLoseCondition()
    {
        // Sebi met la condition de lose ici stp
        return false;
    }

    private void ShowWinScreen()
    {
        winRoot.style.display = DisplayStyle.Flex;
    }

    private void ShowLoseScreen()
    {
        loseRoot.style.display = DisplayStyle.Flex;
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(nextScene);
    }

    private void ExitGame()
    {
        Application.Quit();
    }
}