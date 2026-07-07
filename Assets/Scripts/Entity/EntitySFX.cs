using System;
using Unity.VisualScripting;
using UnityEngine;

public class EntitySFX : MonoBehaviour
{
    [SerializeField] protected bool showDraw = false;
    protected AudioSource audioSource;

    [Header("SFX Name")]
    [SerializeField] private string attackHit;
    [SerializeField] private string attackMiss;
    [SerializeField] private string move;
    [SerializeField] private string death;
    
    [Space]
    [SerializeField] protected float soundDistacne = 15f;

    [Header("Move")]
    [SerializeField] private float moveSFXInterval = 0.35f;
    private float lastMoveSFXTime = -999f;

    private void Awake()
    {
        audioSource = GetComponentInChildren<AudioSource>();
    }

    public void PlayAttackHit()
    {
        AudioManager.instance.PlaySFX(attackHit, audioSource, soundDistacne);
    }

    public void PlayAttackMiss()
    {
        AudioManager.instance.PlaySFX(attackMiss, audioSource, soundDistacne);
    }

    public void PlayMove()
    {
        AudioManager.instance.PlaySFX(move, audioSource, soundDistacne);
    }
    
    public void PlayDeath()
    {
        AudioManager.instance.PlaySFX(death, audioSource, soundDistacne);
    }

    public void TryPlayMove()
    {
        TryPlayMove(moveSFXInterval);
    }    

    public void TryPlayMove(float SFXInterval)
    {
        if (Time.time < lastMoveSFXTime + SFXInterval)
        {
            return;
        }
        lastMoveSFXTime = Time.time;
        PlayMove();
    }

    private void OnDrawGizmos()
    {
        if (showDraw)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, soundDistacne);
        }
        
    }
}
