using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioDatabaseSO audioDatabase;
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    private Transform player;
    private AudioClip lastMusic = null;
    private string currentBGMGroupName;
    private Coroutine currentBGMCo;

    [Space]
    [SerializeField] private bool shouldPlayBGM;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        } 
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (!bgmSource.isPlaying && shouldPlayBGM)
        {
            if (!string.IsNullOrEmpty(currentBGMGroupName))
            {
                NextBGM(currentBGMGroupName);
            }
        }

        if (bgmSource.isPlaying && !shouldPlayBGM)
        {
            StopBGM();
        }
    }

    public void StartBGM(string musicGroup)
    {
        shouldPlayBGM = true;
        if (currentBGMGroupName == musicGroup)
        {
            return;
        }
        NextBGM(musicGroup);
    }

    public void NextBGM(string musicGroup)
    {
        shouldPlayBGM = true;
        currentBGMGroupName = musicGroup;
        if (currentBGMCo != null)
        {
            StopCoroutine(currentBGMCo);
        }
        currentBGMCo = StartCoroutine(SwitchMusicCo(musicGroup));
    }

    public void StopBGM()
    {
        shouldPlayBGM = false;
        StartCoroutine(FadeVolumeCo(bgmSource, 0f, 1f));
        if (currentBGMCo != null)
        {
            StopCoroutine(currentBGMCo);
        }
    }

    private IEnumerator SwitchMusicCo(string musicGroup)
    {
        AudioClipData data = audioDatabase.GetAudioClipData(musicGroup);
        if (data == null || data.clips.Count == 0)
        {
            Debug.Log("No audio in the group" + musicGroup);
            yield break;
        } 
        AudioClip nextMusic = data.GetRandomClip();
        if (data.clips.Count > 1)
        {
            while (nextMusic == lastMusic)
            {
                nextMusic = data.GetRandomClip();
            }
        }
        if (bgmSource.isPlaying)
        {
            yield return FadeVolumeCo(bgmSource, 0f, 1f);
        }

        lastMusic = nextMusic;
        bgmSource.clip = nextMusic;
        bgmSource.volume = 0;
        bgmSource.Play();

        StartCoroutine(FadeVolumeCo(bgmSource, data.maxVolume, 1f));
    }

    private IEnumerator FadeVolumeCo(AudioSource audioSource, float targetVolume, float duration)
    {
        float time = 0f;
        float startVolume = audioSource.volume;

        while(time < duration)
        {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            yield return null;
        }
        audioSource.volume = targetVolume;
    }

    public void PlaySFX(string soundName, AudioSource sfxSource, float minDistanceToHear)
    {
        if (soundName == null)
        {
            Debug.Log("Do not have the sound");
            return;
        }
        if (player == null)
        {
            player = Player.instance.transform;
        }
        AudioClipData data = audioDatabase.GetAudioClipData(soundName);
        if (data == null)
        {
            Debug.Log("Try to play sound" + soundName);
            return;
        }
        AudioClip clip = data.GetRandomClip();
        if (clip == null)
        {
            return;
        }

        float maxVolume = data.maxVolume;
        float distance = Vector2.Distance(sfxSource.transform.position, player.position);
        float t = Mathf.Clamp01(1 - distance / minDistanceToHear);

        sfxSource.clip = clip;
        sfxSource.volume = Mathf.Lerp(0, maxVolume, t * t);
        sfxSource.pitch = UnityEngine.Random.Range(0.95f, 1.1f);
        sfxSource.PlayOneShot(clip);
    }

    public void PlayGlobalSFX(string soundName)
    {
        AudioClipData data = audioDatabase.GetAudioClipData(soundName);
        if (data == null)
        {
            return;
        }
        AudioClip clip = data.GetRandomClip();
        if (clip == null)
        {
            return;
        }
        sfxSource.clip = clip;
        sfxSource.pitch = UnityEngine.Random.Range(0.95f, 1.1f);
        sfxSource.volume = data.maxVolume;
        sfxSource.PlayOneShot(clip);
    }

}
