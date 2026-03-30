using UnityEngine;

[CreateAssetMenu(menuName = "Data/Enemy")]

public class EnemyData : ScriptableObject
{
    [Header("General")]
    public GameObject PREFAB;

    public string NAME;
    public int HEALTH;
    public int DAMAGE;

    public float SPEED_MAX;
    public float SPEED_ACCEL;

    public float VIEW_DISTANCE;

    [Header("Chasing State")]
    public float RANGE;
    public float RANGE_PADDING;

    [Header("Orbiting State")]
    public float DRIFT_OFFSET_ANGLE;
    public float DRIFT_OFFSET_ANGLE_PADDING;
    public float DRIFT_FORCE;
}