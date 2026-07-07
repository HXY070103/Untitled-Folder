using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioDatabaseSO audioDatabase;
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    private Transform player;

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

        float maxVolume = data.volume;
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
        sfxSource.volume = data.volume;
        sfxSource.PlayOneShot(clip);
    }

}
