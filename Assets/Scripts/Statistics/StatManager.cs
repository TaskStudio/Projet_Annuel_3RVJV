using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatManager : MonoBehaviour
{
    public static StatManager Instance { get; private set; }

    public float elapsedTime = 0f;
    private bool isRunning = true;

    public static int allyDeathCount = 0;
    public static int enemyDeathCount = 0;
    public static int allyDamageTaken = 0;
    public static int enemyDamageTaken = 0;
    public static int allyWoodCollected = 0;
    public static int allyStoneCollected = 0;
    public static int allyGoldCollected = 0;
    public static int allyGoldSpent = 0;
    public static int allyStoneSpent = 0;
    public static int allyWoodSpent = 0;
    public static int unitProductionCount = 0;

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

    void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
        }

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
        int milliseconds = Mathf.FloorToInt((elapsedTime * 100F) % 100F);
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

    public static int AllyDamageTaken => allyDamageTaken;
    public static int EnemyDamageTaken => enemyDamageTaken;
    public static int AllyDeathCount => allyDeathCount;
    public static int EnemyDeathCount => enemyDeathCount;

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