using UnityEngine;

public class PointMass
{
    public Vector3 pos;
    public Vector3 velo;
    public float inverseMass;

    private Vector3 acceleration;
    private float damping = 0.98f;

    public PointMass(Vector3 position, float invMass)
    {
        pos = position;
        inverseMass = invMass;
    }

    public void ApplyForce(Vector3 force)
    {
        acceleration += force * inverseMass;
    }

    public void IncreaseDamping(float factor)
    {
        damping *= factor;
    }

    public void UpdatePoint()
    {
        velo += acceleration;
        pos += velo;
        acceleration = Vector3.zero;

        if (velo.sqrMagnitude < 0.000001f)
            velo = Vector3.zero;

        velo *= damping;
        damping = 0.98f;
    }
}

public struct Spring
{
    public PointMass end1;
    public PointMass end2;
    public float targetLength;
    public float stiffness;
    public float damping;

    public Spring(PointMass end1, PointMass end2, float stiffness, float damping)
    {
        this.end1 = end1;
        this.end2 = end2;
        this.stiffness = stiffness;
        this.damping = damping;
        // The problematic part before the camera logic:
        targetLength = Vector3.Distance(end1.pos, end2.pos) * 0.95f;
    }

    public void UpdateSpring()
    {
        Vector3 x = end1.pos - end2.pos;
        float length = x.magnitude;

        // Only pull, not push
        if (length <= targetLength)
            return;

        x = (x / length) * (length - targetLength);
        Vector3 dv = end2.velo - end1.velo;
        Vector3 force = stiffness * x - dv * damping;

        end1.ApplyForce(-force);
        end2.ApplyForce(force);
    }
}

