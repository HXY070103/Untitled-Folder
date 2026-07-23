using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    [SerializeField] private GameObject platform;

    public void HidePlatform()
    {
        platform.SetActive(false);
    }

    public void ShowPlatform()
    {
        platform.SetActive(true);
    }
}
