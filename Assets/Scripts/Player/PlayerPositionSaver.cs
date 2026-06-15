using UnityEngine;

public class PlayerPositionSaver : MonoBehaviour, ISaveable
{
    public void LoadData(GameData data)
    {
        if (!data.hasPlayerPosition)
        {
            return;
        }
        transform.position = new Vector3(data.playerPositionX, data.playerPositionY, data.playerPositionZ);
    }

    public void SaveData(ref GameData data)
    {
        data.hasPlayerPosition = true;

        data.playerPositionX = transform.position.x;
        data.playerPositionY = transform.position.y;
        data.playerPositionZ = transform.position.z;
    }
}