using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Enemy Data")]

public class EnemyData : ScriptableObject
{
    public string NAME;
    public int HEALTH;
    public float SPEED;
    public int DAMAGE;
    public float VIEW_DISTANCE;
    public GameObject PREFAB;
}