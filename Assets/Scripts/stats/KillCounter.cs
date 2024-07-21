using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillCounter : MonoBehaviour
{
    public static int allyDeathCount = 0;
    public static int enemyDeathCount = 0;
    

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
