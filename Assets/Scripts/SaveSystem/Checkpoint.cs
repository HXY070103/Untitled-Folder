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

        Instantiate(lightBeamPrefab, transform.position, Quaternion.identity);

        SaveManager saveManager = FindFirstObjectByType<SaveManager>();

        if (saveManager == null)
        {
            Debug.LogError("Checkpoint: SaveManager not found");
            return;
        }

        saveManager.SetCheckpoint(transform.position);
        saveManager.SaveGame();

    }
}
