using System;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySFX : EntitySFX
{
    [Header("SFX Name for Enemy")]
    [SerializeField] private string specialAttack1;
    [SerializeField] private string specialAttack2;

    public void PlaySpecialAttack1()
    {
        AudioManager.instance.PlaySFX(specialAttack1, audioSource, soundDistacne);
    }

    public void PlaySpecialAttack2()
    {
        AudioManager.instance.PlaySFX(specialAttack2, audioSource, soundDistacne);
    }
}
