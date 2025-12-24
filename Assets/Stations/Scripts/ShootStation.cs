using System;
using Unity.Mathematics;
using UnityEngine;

public class ShootStation : AbstractStation
{
    [Header("Bullet Settings")]
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float bulletSpeed = 10f;
    [SerializeField] float bulletLifetime = 10f;
    [SerializeField] float bulletsPerSecond= 1f;
    [SerializeField] float bulletSpread = 1f;
    float shootCooldown;

    [Header("Rotation")] 
    [SerializeField] float rotationRange;
    [SerializeField] float smoothTime = 0.3f;
    [SerializeField] float smoothMaxSpeed = 0.3f;
    [SerializeField] float rotRange = 90f;
    float startAngle;

    float targetAngle = 0.0f;

    //internal smoothdamp velo
    float angularVelocity = 0.0f;

    void Awake()
    {
        // Initialize the start angle based on the station device's current rotation
        startAngle = NormalizeTo180(Mathf.Round(stationDevice.transform.rotation.eulerAngles.z));

        shootCooldown = 1f / bulletsPerSecond;
    }

    override public void StationAction(bool isPressedThisFrame)
    {
        if (shootCooldown < 0)
        {
            GameObject bullet = Instantiate(
                bulletPrefab,
                stationDevice.transform.position,
                Quaternion.Euler(0, 0, stationDevice.transform.rotation.eulerAngles.z + UnityEngine.Random.Range(-bulletSpread, bulletSpread))
            );

            bullet.GetComponent<Bullet>().bulletSpeed = bulletSpeed;
            bullet.GetComponent<Bullet>().bulletLifetime = bulletLifetime;

            shootCooldown = 1f / bulletsPerSecond;
        }
    }

    override public void StationUpdateDir(Vector2 dir)
    {
        if (dir.magnitude < 0.4f)
        {
            // If the direction is too small, do not rotate
            return;
        }

        targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        targetAngle = ClampAngleRelative(
            targetAngle,
            startAngle,
            rotRange
        );


        RayAngle(targetAngle);
    }

    protected override void Update()
    {        
        base.Update();
        shootCooldown -= Time.deltaTime;

        // Get current z rotation (absolute
        float currentAngle = stationDevice.transform.eulerAngles.z;

        // Smooth toward target
        float smoothedAngle = Mathf.SmoothDampAngle(
            currentAngle,
            targetAngle,
            ref angularVelocity,
            smoothTime,
            smoothMaxSpeed
        );


        // Apply back
        stationDevice.transform.rotation = Quaternion.Euler(0, 0, smoothedAngle);
    }

    void RayAngle(float angle)
    {
        Debug.DrawRay(stationDevice.transform.position, Quaternion.Euler(0, 0, angle) * Vector2.right * 10f, Color.red);
    }
    
    float NormalizeTo180(float angle)
    {
        angle %= 360f;          // now in -360 → 360
        if (angle > 180f) 
            angle -= 360f;      // shift down into -180 → 180
        else if (angle < -180f) 
            angle += 360f;      // shift up into -180 → 180
        return angle;
    }

    float ClampAngleRelative(float angle, float center, float halfRange)
    {
        // Normalize everything relative to the center
        float relative = NormalizeTo180(angle - center);

        // If inside the range → keep as-is
        if (Mathf.Abs(relative) <= halfRange)
            return angle;

        // If outside → snap to whichever bound is closer
        float snapped = (relative > 0) ? halfRange : -halfRange;

        // Convert back into world space
        return NormalizeTo180(center + snapped);
    }
}
