using System;
using Unity.Mathematics;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [HideInInspector] public float bulletLifetime = 2f;
    [HideInInspector] public float bulletSpeed = 2f;

    void Start()
    {
        Destroy(gameObject, bulletLifetime);
    }

    void Update()
    {
        transform.Translate(Vector2.right * bulletSpeed * Time.deltaTime);
    }
}
