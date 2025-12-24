using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Enemy Data")]

public class EnemyData : ScriptableObject
{
    public string enemyName;
    public int health;
    public float speed;
    public int damage;
    public GameObject enemyPrefab;
}