using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSFX : EntitySFX
{
    [Header("SFX Name for Player")]
    [SerializeField] private string dash;
    [SerializeField] private string jump;
    [SerializeField] private string jumpland;

    public void PlayDash()
    {
        AudioManager.instance.PlaySFX(dash, audioSource, soundDistacne);
    }
    
    public void PlayJump()
    {
        AudioManager.instance.PlaySFX(jump, audioSource, soundDistacne);
    }

    public void PlayJumpland()
    {
        AudioManager.instance.PlaySFX(jumpland, audioSource, soundDistacne);
    }
}
