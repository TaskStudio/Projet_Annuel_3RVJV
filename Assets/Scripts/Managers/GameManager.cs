using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    [SerializeField] private string nextScene;
    [SerializeField] private UIDocument winUIDocument;
    [SerializeField] private UIDocument loseUIDocument;

    [FormerlySerializedAs("playerEntityDatabase")]
    public UnitDatabaseSO playerUnitDatabase;
    public BuildingDatabaseSO playerBuildingDatabase;

    private VisualElement winRoot;
    private VisualElement loseRoot;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

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
        return false;
    }

    private bool CheckLoseCondition()
    {
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