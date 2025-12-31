using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class EnemyManager
{
    public static List<GameObject> idleEnemies { get; private set; }
    public static List<GameObject> chasingEnemies { get; private set; }
    
    static EnemyManager() 
    {
        idleEnemies = new List<GameObject>();
        chasingEnemies = new List<GameObject>();
    }

    public enum EnemyState
    {
        Idle,
        Chasing
    }

    public static void AddEnemy(GameObject enemy, EnemyState state)
    {
        if (state == EnemyState.Idle)
            idleEnemies.Add(enemy);
        else if (state == EnemyState.Chasing)
            chasingEnemies.Add(enemy);
    }

    public static void RemoveEnemy(GameObject enemy, EnemyState state)
    {
        if (state == EnemyState.Idle)
            idleEnemies.Remove(enemy);
        else if (state == EnemyState.Chasing)
            chasingEnemies.Remove(enemy);
    }
}
