using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.Leaderboards;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    [SerializeField] private string replayScene;
    [SerializeField] private string menuScene;
    [SerializeField] private GameUIManager gameUIManager;

    private readonly List<Nexus> allyNexusList = new();
    private readonly List<Nexus> enemyNexusList = new();

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void Start()
    {
        // Find all Nexus objects in the scene and categorize them
        var nexusArray = FindObjectsOfType<Nexus>();
        foreach (var nexus in nexusArray)
            if (nexus.CompareTag("AllyBase"))
                allyNexusList.Add(nexus);
            else if (nexus.CompareTag("EnemyBase")) enemyNexusList.Add(nexus);
    }

    private void Update()
    {
        if (CheckWinCondition())
            gameUIManager.ShowWinScreen();
        else if (CheckLoseCondition()) gameUIManager.ShowLoseScreen();
    }

    private bool CheckWinCondition()
    {
        // Win if all enemy Nexus objects are destroyed
        return enemyNexusList.Count == 0;
    }

    private bool CheckLoseCondition()
    {

        // Lose if all ally Nexus objects are destroyed
        return allyNexusList.Count == 0;
        
    } // ReSharper disable Unity.PerformanceAnalysis
    public void OnNexusDestroyed(Nexus nexus)
    {
        if (nexus.CompareTag("AllyBase"))
            allyNexusList.Remove(nexus);
        else if (nexus.CompareTag("EnemyBase")) enemyNexusList.Remove(nexus);

        // Check game state immediately after a Nexus is destroyed
        if (CheckWinCondition())
            gameUIManager.ShowWinScreen();
        else if (CheckLoseCondition())
        {
            gameUIManager.ShowLoseScreen();
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(replayScene);
    }

    public void ExitGame()
    {
        SceneManager.LoadScene(menuScene);
    }
    public void LeaderBoard()
    {
        int score = (StatManager.GetAllyGoldCollected() + StatManager.GetAllyStoneCollected() +
                     StatManager.GetAllyWoodCollected() + StatManager.EnemyDeathCount + StatManager.EnemyDamageTaken) /
                    (int)StatManager.elapsedTime;
    
        var options = new AddPlayerScoreOptions
        {
            Metadata = new Dictionary<string, string>
            {
                {"TimeElapsed", StatManager.Instance.GetFormattedElapsedTime()},
                {"EntitiesKilled", StatManager.EnemyDeathCount.ToString()},
                {"EntitiesLost", StatManager.AllyDeathCount.ToString()},
                {"EntitiesCreated", StatManager.GetUnitProductionCount().ToString()},
                {"DamageDealt", StatManager.EnemyDamageTaken.ToString()},
                {"DamageReceived", StatManager.AllyDamageTaken.ToString()},
                {"ResourcesCollected", (StatManager.GetAllyWoodCollected() + StatManager.GetAllyStoneCollected() + StatManager.GetAllyGoldCollected()).ToString()},
                {"WoodCollected", StatManager.GetAllyWoodCollected().ToString()},
                {"StoneCollected", StatManager.GetAllyStoneCollected().ToString()},
                {"GoldCollected", StatManager.GetAllyGoldCollected().ToString()},
                {"ResourcesUsed", (StatManager.GetAllyWoodSpent() + StatManager.GetAllyStoneSpent() + StatManager.GetAllyGoldSpent()).ToString()},
                {"WoodUsed", StatManager.GetAllyWoodSpent().ToString()},
                {"StoneUsed", StatManager.GetAllyStoneSpent().ToString()},
                {"GoldUsed", StatManager.GetAllyGoldSpent().ToString()}
            }
        };

        // Add player score to leaderboard with the options instance
        LeaderboardsService.Instance.AddPlayerScoreAsync("Leaderboard", score, options);
    }
   
}