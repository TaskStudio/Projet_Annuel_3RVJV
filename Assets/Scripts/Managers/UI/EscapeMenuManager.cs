using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class EscapeMenuManager : MonoBehaviour
{
    [SerializeField] private UIDocument escapeMenuDocument;
    [SerializeField] private string mainMenuScene;

    private VisualElement escapeMenuRoot;
    private bool isGamePaused;
    private Button pauseButton;

    private void Start()
    {
        if (escapeMenuDocument != null)
        {
            escapeMenuRoot = escapeMenuDocument.rootVisualElement;
            escapeMenuRoot.style.display = DisplayStyle.None;

            var resumeButton = escapeMenuRoot.Q<Button>("Resume");
            pauseButton = escapeMenuRoot.Q<Button>("Pause");
            var exitButton = escapeMenuRoot.Q<Button>("Exit");

            resumeButton.clicked += ResumeGame;
            pauseButton.clicked += PauseGame;
            exitButton.clicked += ExitGame;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (escapeMenuRoot.style.display == DisplayStyle.None)
                OpenEscapeMenu();
            else
                CloseEscapeMenu();
        }
    }

    private void OpenEscapeMenu()
    {
        escapeMenuRoot.style.display = DisplayStyle.Flex;
        // Do not pause the game when the menu is opened
    }

    private void CloseEscapeMenu()
    {
        escapeMenuRoot.style.display = DisplayStyle.None;
        if (isGamePaused)
            Time.timeScale = 0f; // Keep the game paused if it's paused
        else
            Time.timeScale = 1f; // Resume the game if it's not paused
    }

    private void ResumeGame()
    {
        isGamePaused = false;
        Time.timeScale = 1f; // Resume the game
        pauseButton.RemoveFromClassList("buttonsClicked");
        pauseButton.AddToClassList("buttons");
        CloseEscapeMenu();
    }

    private void PauseGame()
    {
        isGamePaused = true;
        Time.timeScale = 0f; // Pause the game
        pauseButton.RemoveFromClassList("buttons");
        pauseButton.AddToClassList("buttonsClicked");
    }

    private void ExitGame()
    {
        Time.timeScale = 1f; // Ensure time scale is reset before exiting
        SceneManager.LoadScene(mainMenuScene);
    }
}