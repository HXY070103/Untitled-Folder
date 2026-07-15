using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UI_Options : MonoBehaviour
{
    private Player player;

    [SerializeField] private Toggle healthBarToggle;
    
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private float mixerMultiplier = 25;

    [Header("BGM Volume Settings")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private string bgmParameter;

    [Header("SFX Volume Settings")]
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private string sfxParameter;

    private void Awake()
    {
        player = FindFirstObjectByType<Player>();
        healthBarToggle.onValueChanged.AddListener(OnHealthBarToggleChanged);
    }

    private void OnHealthBarToggleChanged(bool isOn)
    {
        AudioManager.instance.PlayGlobalSFX("Button_Click");
        player.health.EnableHealthBar(isOn);
    }

    private void OnEnable()
    {
        sfxSlider.value = PlayerPrefs.GetFloat(sfxParameter, 0.6f);
        bgmSlider.value = PlayerPrefs.GetFloat(bgmParameter, 0.6f);
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat(sfxParameter, sfxSlider.value);
        PlayerPrefs.SetFloat(bgmParameter, bgmSlider.value);
    }

    public void LoadUpVolume()
    {
        sfxSlider.value = PlayerPrefs.GetFloat(sfxParameter, 0.6f);
        bgmSlider.value = PlayerPrefs.GetFloat(bgmParameter, 0.6f);
    }

    public void BGMSliderValue(float value)
    {
        float newValue = Mathf.Log10(value) * mixerMultiplier;
        audioMixer.SetFloat(bgmParameter, newValue);
    }

    public void SFXSliderValue(float value)
    {
        float newValue = Mathf.Log10(value) * mixerMultiplier;
        audioMixer.SetFloat(sfxParameter, newValue);
    }

    public void SaveAndExitToMainMenu()
    {
        AudioManager.instance.PlayGlobalSFX("Button_Click");
        if (SaveManager.instance != null)
        {
            SaveManager.instance.SaveGameIfAllowed();
        }

        Time.timeScale = 1f;
        if (HistoryManager.instance != null)
        {
            HistoryManager.instance.RecordCurrentStatsBeforeExit();
        }

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
            GameManager.instance.LoadScene("Mainscene");
        }
        else
        {
            Debug.LogError("GameManager not found");
        }
    }
}
