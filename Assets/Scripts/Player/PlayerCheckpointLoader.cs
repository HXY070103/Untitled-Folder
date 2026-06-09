using UnityEngine;

public class PlayerCheckpointLoader : MonoBehaviour, ISaveable
{
    public void LoadData(GameData data)
    {
        if (!data.hasCheckpoint)
        {
            return;
        }
        transform.position = new Vector3(data.checkpointX, data.checkpointY, data.checkpointZ);
    }

    public void SaveData(ref GameData data)
    {
        
    }
}