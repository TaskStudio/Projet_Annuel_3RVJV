using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float elapsedTime = 0f;
    private bool isRunning = true;
    

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
        return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
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
}