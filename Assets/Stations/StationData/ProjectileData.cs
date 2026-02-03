using UnityEngine;

[CreateAssetMenu(menuName = "Data/Projectile")]

public class ProjectileData : ScriptableObject
{
    [Header("General")]
    public int DAMAGE;
    public GameObject PARTICLE_ON_HIT;
}
