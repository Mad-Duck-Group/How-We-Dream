using System.Collections.Generic;
using Microlight.MicroAudio;
using UnityCommunity.UnitySingleton;
using UnityEngine;
using UnityRandom = UnityEngine.Random;

public class GlobalSoundManager : MonoSingleton<GlobalSoundManager>
{
    private record AudioSourceData(string Id, AudioSource AudioSource)
    {
         public string Id { get; } = Id;
         public AudioSource AudioSource { get; } = AudioSource;
    }
    [SerializeField] private AudioSO audioSO;
    private List<AudioSourceData> audioSourceData = new();

    public void PlayBGM(string audioName, bool crossfade = true, float duration = 1f)
    {
        var clip = audioSO.GetAudioClip(audioName);
        if (crossfade && MicroAudio.MusicAudioSource.isPlaying)
        {
            var beforeVolume = MicroAudio.MusicAudioSource.volume;
            var fade = new SoundFade(MicroAudio.MusicAudioSource, MicroAudio.MusicAudioSource.volume, 0f, duration);
            fade.OnFadeEnd += _ =>
            {
                MicroAudio.StopMusic();
                MicroAudio.MusicAudioSource.volume = beforeVolume;
                MicroAudio.PlayOneTrack(clip, crossfade: duration);
            };
        }
        else
        {
            MicroAudio.PlayOneTrack(clip, crossfade: duration);
        }
    }

    public void StopBGM(bool fade = true, float duration = 1f)
    {
        if (fade)
        {
            var beforeVolume = MicroAudio.MusicAudioSource.volume;
            var soundFade = new SoundFade(MicroAudio.MusicAudioSource, MicroAudio.MusicAudioSource.volume, 0f, duration);
            soundFade.OnFadeEnd += _ =>
            {
                MicroAudio.StopMusic();
                MicroAudio.MusicAudioSource.volume = beforeVolume;
            };
        }
        else
        {
            MicroAudio.StopMusic();
        }
    }

    public void PlayUISFX(string audioName, bool loop = false, string id = "")
    {
        var clip = audioSO.GetAudioClip(audioName);
        var audioSource = MicroAudio.PlayUISound(clip, loop: loop);
        if (!string.IsNullOrEmpty(id)) audioSourceData.Add(new AudioSourceData(id, audioSource));
    }
    
    public void StopSound(string id)
    {
        var data = audioSourceData.Find(data => data.Id == id);
        if (data == null) return;
        if (data.AudioSource == null)
        {
            audioSourceData.Remove(data);
            return;
        }
        data.AudioSource.Stop();
        data.AudioSource.loop = false;
        data.AudioSource.clip = null;
        audioSourceData.Remove(data);
    }

    public void PlayEffectClip(string audioName)
    {
        var clip = audioSO.GetAudioClip(audioName);
        MicroAudio.PlayEffectSound(clip);
    }
    
    public void ChangeVolume(float volume)
    {
        MicroAudio.MasterVolume = volume;
    }
}