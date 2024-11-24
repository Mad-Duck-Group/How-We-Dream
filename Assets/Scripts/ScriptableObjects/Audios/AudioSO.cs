using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "Audio", menuName = "ScriptableObjects/Audio")]
public class AudioSO : ScriptableObject
{
    [SerializedDictionary("Name", "Audio Clip")]
    public SerializedDictionary<string, AudioClip> audioData;
    
    public AudioClip GetAudioClip(string audioName)
    {
        if (audioData.TryGetValue(audioName, out var clip)) return clip;
        Debug.LogError($"Audio with name {audioName} not found, make sure it's in the AudioSO and spelled correctly.");
        return null;
    }
}
