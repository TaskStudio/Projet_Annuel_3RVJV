using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatManager : MonoBehaviour
{ 
    public float elapsedTime = 0f;
    private bool isRunning = true;
    public static int allyDeathCount = 0;
    public static int enemyDeathCount = 0;
    

    void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
        }
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
    public static int AllyDeathCount
    {
        get { return allyDeathCount; }
    }

    public static int EnemyDeathCount
    {
        get { return enemyDeathCount; }
    }

    public static void IncrementAllyDeathCount()
    {
        allyDeathCount++;
    }

    public static void IncrementEnemyDeathCount()
    {
        enemyDeathCount++;
    }
}
