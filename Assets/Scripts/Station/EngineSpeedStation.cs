using System;
using Unity.Mathematics;
using UnityEngine;

public class EngineSpeedStation : AbstractStation
{
    [Header("Engine Rotation Settings")]
    [SerializeField] float MoveSpeed = 5.0f;
    [SerializeField] float SpeedCap = 5.0f;
    [SerializeField] Rigidbody2D shipRB;
    [SerializeField] Transform thrusterTransform;

    override public void StationUpdateDir(Vector2 dir)
    {

    }
    
    override public void StationAction()
    {
        
    }
    

    protected override void Update()
    {
        base.Update();

        
        Vector2 forceVector = (Mathf.Cos(thrusterTransform.rotation.eulerAngles.z * Mathf.Deg2Rad) * Vector2.right +
                               Mathf.Sin(thrusterTransform.rotation.eulerAngles.z * Mathf.Deg2Rad) * Vector2.up) * MoveSpeed;
        forceVector *= -1;

        shipRB.AddForce(forceVector, ForceMode2D.Force);
        
        if (shipRB.linearVelocity.magnitude > SpeedCap)
        {
            shipRB.linearVelocity = shipRB.linearVelocity.normalized * SpeedCap;
        }
    }
}
