using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class EngineSpeedStation : AbstractStation
{
    [Header("Engine Rotation Settings")]
    [SerializeField] float MOVE_SPEED = 5.0f;
    [SerializeField] float SPEED_CAP = 5.0f;
    [SerializeField] Rigidbody2D SHIP_RB;
    [SerializeField] Transform THRUSTER_TRANSFORM;

    [SerializeField] SpriteRenderer[] GEAR_INDICATORS;
    [SerializeField] Color[] GEAR_COLORS;

    [SerializeField] int currentGear = 0;

    [SerializeField] VisualEffect vfx;
    
    override public void StationUpdateDir(Vector2 dir) {}

    public override void StationAction(bool isPressedThisFrame)
    {
        if (isPressedThisFrame)
        {
            currentGear++;
            currentGear %= GEAR_INDICATORS.Length + 1;
        }

        for (int i = 0; i < GEAR_INDICATORS.Length; i++)
        {
            GEAR_INDICATORS[i].color = Color.black;

            if (i < currentGear)
                GEAR_INDICATORS[i].color = GEAR_COLORS[currentGear];
        }
    }


    protected override void Update()
    {
        base.Update();

        vfx.SetFloat("EngineSpeed", currentGear);

        float angle = (THRUSTER_TRANSFORM.rotation.eulerAngles.z + 90) * Mathf.Deg2Rad;
        Vector2 forceVector = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * MOVE_SPEED * Time.deltaTime;

        forceVector *= -1;
        
        forceVector *= currentGear / 3.0f;

        SHIP_RB.AddForce(forceVector * SHIP_RB.mass, ForceMode2D.Force);

        if (SHIP_RB.linearVelocity.magnitude > SPEED_CAP)
        {
            SHIP_RB.linearVelocity = SHIP_RB.linearVelocity.normalized * SPEED_CAP;
        }
    }
}
