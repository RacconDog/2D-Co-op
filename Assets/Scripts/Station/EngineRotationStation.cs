using System;
using Unity.Mathematics;
using UnityEngine;

public class EngineRotationStation : AbstractStation
{
    [Header("Engine Rotation Settings")]
    [SerializeField] float smoothTime = 0.3f;
    [SerializeField] float smoothMaxSpeed = 0.3f;

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
    
    override public void StationAction()
    {
        
    }

    protected override void Update()
    {
        base.Update();

        // Get current z rotation (absolute)
        float currentAngle = stationDevice.transform.eulerAngles.z;

        // Smooth toward target
        float smoothedAngle = Mathf.SmoothDampAngle(
            currentAngle,
            targetAngle - 90f,
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
}
