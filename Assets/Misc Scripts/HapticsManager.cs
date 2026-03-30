using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;

public class HapticsManager : MonoBehaviour
{
    [SerializeField] private float hueSpeed = 0.2f;

    [SerializeField] AnimationCurve highFreq;
    [SerializeField] AnimationCurve lowFreq;

    [SerializeField] float duration = 1f;

    [SerializeField] float internalTimer = 0f;

    void Update()
    {
        SetLightBar(Color.HSVToRGB(Time.time * hueSpeed % 1, 1, 1));

        internalTimer -= Time.deltaTime;

        if (Gamepad.current is DualSenseGamepadHID dualSense)
        {
            if (internalTimer > 0)
            {
                dualSense.SetMotorSpeeds(
                    lowFreq.Evaluate(internalTimer / duration),
                    highFreq.Evaluate(internalTimer / duration)
                );
            }
            else
            {
                dualSense.SetMotorSpeeds(
                    0f,
                    0f
                );
            }
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            HitVibrate();
        }
    }

    public void HitVibrate()
    {
        internalTimer = duration;
    }

    void SetLightBar(Color color)
    {
        if (Gamepad.current is DualSenseGamepadHID dualSense)
        {
            dualSense.SetLightBarColor(color);
        }
    }
}
