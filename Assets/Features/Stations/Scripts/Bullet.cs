using System;
using Unity.Mathematics;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [HideInInspector] public float BULLET_LIFETIME = 2f;
    [HideInInspector] public float BULLET_SPEED = 2f;

    [SerializeField] public ProjectileData bulletData;

    void Start()
    {
        Destroy(gameObject, BULLET_LIFETIME);
    }

    void Update()
    {
        transform.Translate(Vector2.right * BULLET_SPEED * Time.deltaTime);
    }
}
