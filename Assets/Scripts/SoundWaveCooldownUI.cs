using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SoundWaveCooldownUI : MonoBehaviour
{
    [SerializeField] private Image foregroundBar;
    [SerializeField] private float cooldownDuration = 1f;

    private float cooldownTimer = 0f;
    private bool isCoolingDown = false;

    void Update()
    {
        // Handle cooldown progress
        if (isCoolingDown)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= cooldownDuration)
            {
                cooldownTimer = cooldownDuration;
                isCoolingDown = false;
            }

            // Fill up the bar from 0 to 1
            foregroundBar.fillAmount = cooldownTimer / cooldownDuration;
        }
    }

    // Called by Boat when soundwave is used
    public void StartCooldown()
    {
        cooldownTimer = 0f;
        isCoolingDown = true;
        foregroundBar.fillAmount = 0f;
    }

    public void SetCooldownDuration(float duration)
    {
        cooldownDuration = duration;
    }

    public bool IsReady()
    {
        return !isCoolingDown;
    }
}


