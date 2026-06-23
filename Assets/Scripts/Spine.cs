using UnityEngine;

public class Spine : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() == null)
        {
            return;
        }
        PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
        if (playerHealth == null)
        {
            Debug.Log("Cannot find player health");
            return;
        }
        playerHealth.TryApplyInstantKill();
    }
}