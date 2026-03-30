using System.Collections.Generic;
using UnityEngine;

public class EnemyDebugManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> idleEnemies;
    [SerializeField] private List<GameObject> chasingEnemies;

    void Update()
    {
        idleEnemies = EnemyManager.idleEnemies;
        chasingEnemies = EnemyManager.chasingEnemies;
    }
}
