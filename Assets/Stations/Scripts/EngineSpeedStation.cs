using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;

public class EngineSpeedStation : AbstractStation
{
    [Header("Engine Rotation Settings")]
    [SerializeField] float MoveSpeed = 5.0f;
    [SerializeField] float SpeedCap = 5.0f;
    [SerializeField] Rigidbody2D shipRB;
    [SerializeField] Transform thrusterTransform;

    [SerializeField] SpriteRenderer[] gearIndicators;
    [SerializeField] Color[] gearColors;

    [SerializeField] int currentGear = 0;
    
    override public void StationUpdateDir(Vector2 dir) {}

    public override void StationAction(bool isPressedThisFrame)
    {
        if (isPressedThisFrame)
        {
            currentGear++;
            currentGear %= gearIndicators.Length + 1;
        }

        for (int i = 0; i < gearIndicators.Length; i++)
        {
            gearIndicators[i].color = Color.black;

            if (i < currentGear)
                gearIndicators[i].color = gearColors[currentGear];
        }
    }


    protected override void Update()
    {
        base.Update();

        float angle = (thrusterTransform.rotation.eulerAngles.z + 90) * Mathf.Deg2Rad;
        Vector2 forceVector = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * MoveSpeed * Time.deltaTime;

        forceVector *= -1;
        
        forceVector *= currentGear / 3.0f;

        shipRB.AddForce(forceVector * shipRB.mass, ForceMode2D.Force);

        if (shipRB.linearVelocity.magnitude > SpeedCap)
        {
            shipRB.linearVelocity = shipRB.linearVelocity.normalized * SpeedCap;
        }
    }
}
