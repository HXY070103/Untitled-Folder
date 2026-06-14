using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI_Options : MonoBehaviour
{
    private Player player;

    [SerializeField] private Toggle healthBarToggle;

    private void Awake()
    {
        player = FindFirstObjectByType<Player>();
        healthBarToggle.onValueChanged.AddListener(OnHealthBarToggleChanged);
    }

    private void OnHealthBarToggleChanged(bool isOn)
    {
        player.health.EnableHealthBar(isOn);
    }

    public void SaveAndExitToMainMenu()
    {
        if (SaveManager.instance != null)
        {
            SaveManager.instance.SaveGame();
        }

        Time.timeScale = 1f;

        if (GamePauseManager.Instance != null)
        {
            GamePauseManager.Instance.Resume("UI_Input");
            GamePauseManager.Instance.Resume("SkillChoose");
            GamePauseManager.Instance.Resume("ChestReward");
            GamePauseManager.Instance.Resume("Options");
        }

        if (SaveManager.instance != null)
        {
            SaveManager.instance.DestorySelf();
        }
        if (GameManager.instance != null)
        {
            GameManager.instance.DestorySelf();
        }
        SceneManager.LoadScene("Mainscene");
    }
}
