using System;
using UnityEngine;

public class Spine : MonoBehaviour
{
    [SerializeField] private float cooldown = 1f;
    private float lastTime = -1f;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Time.time < lastTime + cooldown)
        {
            return;
        }
        lastTime = Time.time;
        EntityHealth entityHealth = collision.GetComponent<EntityHealth>();
        EntityStats entityStats = collision.GetComponent<EntityStats>();
        if (entityHealth == null)
        {
            Debug.Log("Cannot find object health");
            return;
        }
        entityHealth.TakeDamage(new Damage(0.2f * entityStats.GetMaxHP(), false), this.transform);
    }
}