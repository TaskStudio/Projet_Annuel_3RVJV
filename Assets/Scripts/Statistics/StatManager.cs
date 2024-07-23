using UnityEngine;

public class StatManager : MonoBehaviour
{
    public static int allyDeathCount;
    public static int enemyDeathCount;
    public static int allyDamageTaken;
    public static int enemyDamageTaken;
    public static int allyWoodCollected;
    public static int allyStoneCollected;
    public static int allyGoldCollected;
    public static int allyGoldSpent;
    public static int allyStoneSpent;
    public static int allyWoodSpent;
    public static int unitProductionCount;

    public float elapsedTime;
    private bool isRunning = true;
    public static StatManager Instance { get; private set; }

    public static int AllyDamageTaken => allyDamageTaken;
    public static int EnemyDamageTaken => enemyDamageTaken;
    public static int AllyDeathCount => allyDeathCount;
    public static int EnemyDeathCount => enemyDeathCount;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void Update()
    {
        if (isRunning) elapsedTime += Time.deltaTime;

        LogAllStats();
    }

    public void LogAllStats()
    {
        // Debug logs for stats
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }

    public string GetFormattedElapsedTime()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60F);
        int seconds = Mathf.FloorToInt(elapsedTime % 60F);
        int milliseconds = Mathf.FloorToInt(elapsedTime * 100F % 100F);
        return $"{minutes:00}:{seconds:00}:{milliseconds:00}";
    }

    public void StartTimer()
    {
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResetTimer()
    {
        elapsedTime = 0f;
    }

    public static void IncrementAllyDeathCount()
    {
        allyDeathCount++;
    }

    public static void IncrementEnemyDeathCount()
    {
        enemyDeathCount++;
    }

    public static void IncrementAllyDamageTaken(int damage)
    {
        allyDamageTaken += damage;
    }

    public static void IncrementEnemyDamageTaken(int damage)
    {
        enemyDamageTaken += damage;
    }

    public static void IncrementResources(Resource resource)
    {
        switch (resource.type)
        {
            case Resource.Type.Wood:
                allyWoodCollected += resource.amount;
                break;
            case Resource.Type.Stone:
                allyStoneCollected += resource.amount;
                break;
            case Resource.Type.Gold:
                allyGoldCollected += resource.amount;
                break;
        }
    }

    public static void IncrementAllyResourcesSpent(Resource resource)
    {
        switch (resource.type)
        {
            case Resource.Type.Wood:
                allyWoodSpent += resource.amount;
                break;
            case Resource.Type.Stone:
                allyStoneSpent += resource.amount;
                break;
            case Resource.Type.Gold:
                allyGoldSpent += resource.amount;
                break;
        }
    }

    public static void IncrementUnitProductionCount()
    {
        unitProductionCount++;
    }

    public static int GetUnitProductionCount()
    {
        return unitProductionCount;
    }

    public static int GetAllyWoodCollected()
    {
        return allyWoodCollected;
    }

    public static int GetAllyStoneCollected()
    {
        return allyStoneCollected;
    }

    public static int GetAllyGoldCollected()
    {
        return allyGoldCollected;
    }

    public static int GetAllyGoldSpent()
    {
        return allyGoldSpent;
    }

    public static int GetAllyStoneSpent()
    {
        return allyStoneSpent;
    }

    public static int GetAllyWoodSpent()
    {
        return allyWoodSpent;
    }
}