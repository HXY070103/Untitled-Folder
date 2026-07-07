using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Audio Database")]
public class AudioDatabaseSO : ScriptableObject
{
    public List<AudioClipData> player;
    public List<AudioClipData> enemies;
    public List<AudioClipData> uiAudio;

    private Dictionary<string, AudioClipData> clipCollection;

    private void OnEnable()
    {
        clipCollection = new Dictionary<string, AudioClipData>();

        AddToCollection(player);
        AddToCollection(enemies);
        AddToCollection(uiAudio);
    }

    public AudioClipData GetAudioClipData(string groupName)
    {
        return clipCollection.TryGetValue(groupName, out var data) ? data : null;
    }

    private void AddToCollection(List<AudioClipData> listToAdd)
    {
        foreach(var data in listToAdd)
        {
            if (data != null && !clipCollection.ContainsKey(data.audioName))
            {
                clipCollection.Add(data.audioName, data);
            }
        }
    }
}


[System.Serializable]
public class AudioClipData
{
    public string audioName;
    public List<AudioClip> clips = new List<AudioClip>();
    [Range(0f, 1f)]
    public float volume = 1f;

    public AudioClip GetRandomClip()
    {
        if (clips == null || clips.Count == 0)
        {
            return null;
        }
        return clips[Random.Range(0, clips.Count)];
    }
}