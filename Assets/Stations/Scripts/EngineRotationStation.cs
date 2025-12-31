using System;
using Unity.Mathematics;
using UnityEngine;

public class EngineRotationStation : AbstractStation
{
    [Header("Engine Rotation Settings")]
    [SerializeField] float SMOOTH_TIME = 0.3f;
    [SerializeField] float SMOOTH_MAX_SPEED = 0.3f;

    float targetAngle = 0.0f;

    //internal smoothdamp velo
    float angularVelocity = 0.0f;

    override public void StationUpdateDir(Vector2 dir)
    {
        if (dir.magnitude < 0.4f)
        {
            // If the direction is too small, do not rotate, kinda like deadzone
            return;
        }

        targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        RayAngle(targetAngle);
    }

    override public void StationAction(bool isPressedThisFrame)
    {
        
    }

    protected override void Update()
    {
        base.Update();

        // Get current z rotation (absolute)
        float currentAngle = STATION_DEVICE.transform.eulerAngles.z;

        // Smooth toward target
        float smoothedAngle = Mathf.SmoothDampAngle(
            currentAngle,
            targetAngle - 90f,
            ref angularVelocity,
            SMOOTH_TIME,
            SMOOTH_MAX_SPEED
        );

        // Apply back
        STATION_DEVICE.transform.rotation = Quaternion.Euler(0, 0, smoothedAngle);
    }

    void RayAngle(float angle)
    {
        Debug.DrawRay(STATION_DEVICE.transform.position, Quaternion.Euler(0, 0, angle) * Vector2.right * 10f, Color.red);
    }
}
