using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public string nextScene;
    public UIDocument winUIDocument;
    public UIDocument loseUIDocument;

    private VisualElement winRoot;
    private VisualElement loseRoot;

    private void Start()
    {
        winRoot = winUIDocument.rootVisualElement;
        loseRoot = loseUIDocument.rootVisualElement;

        winRoot.style.display = DisplayStyle.None;
        loseRoot.style.display = DisplayStyle.None;

        var winRestartButton = winRoot.Q<Button>("restartButton");
        var winExitButton = winRoot.Q<Button>("exitButton");

        var loseRestartButton = loseRoot.Q<Button>("restartButton");
        var loseExitButton = loseRoot.Q<Button>("exitButton");

        winRestartButton.clicked += RestartGame;
        winExitButton.clicked += ExitGame;

        loseRestartButton.clicked += RestartGame;
        loseExitButton.clicked += ExitGame;
    }

    private void Update()
    {
        if (CheckWinCondition())
        {
            ShowWinScreen();
        }
        else if (CheckLoseCondition())
        {
            ShowLoseScreen();
        }
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
