using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private GameObject lightBeamPrefab;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        if (lightBeamPrefab != null)
        {
            Instantiate(lightBeamPrefab, transform.position, Quaternion.identity);
        }
        if (SaveManager.instance == null)
        {
            Debug.LogError("Checkpoint: SaveManager not found");
            return;
        }
        SaveManager.instance.SaveGameIfAllowed();
    }
}
