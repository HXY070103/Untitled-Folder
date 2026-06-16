using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPositionSaver : MonoBehaviour, ISaveable
{
    public void LoadData(GameData data)
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (!data.hasPlayerPosition)
        {
            return;
        }
        if (!data.clearedSceneNames.Contains(currentSceneName))
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