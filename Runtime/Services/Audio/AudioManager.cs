using System.Collections.Generic;
using UnityEngine;
using Speedup.Core;

namespace Speedup.Services.Audio
{
    /// <summary>
    /// Basic audio implementation using Unity's built-in AudioSources.
    /// Derived from MonoBehaviour because it needs AudioSource components.
    /// </summary>
    public class AudioManager : MonoBehaviour, IAudioService
    {
        private AudioSource _musicSource;
        private AudioSource _sfxSource;

        [SerializeField]
        private List<SoundDatabase> _soundDatabases = new List<SoundDatabase>();

        public void Initialize(List<SoundDatabase> soundDatabases)
        {
            _soundDatabases = soundDatabases;
            // Setup the required AudioSources
            _musicSource = gameObject.AddComponent<AudioSource>();
            _musicSource.playOnAwake = false;

            _sfxSource = gameObject.AddComponent<AudioSource>();
            _sfxSource.playOnAwake = false;

            Debug.Log("[AudioManager] Initialized.");
        }

        private AudioClip FindClip(string id)
        {
            foreach (var db in _soundDatabases)
            {
                var clip = db.GetClip(id);
                if (clip != null)
                {
                    return clip;
                }
            }
            return null;
        }

        public void PlayMusic(AudioClip clip, bool loop = true)
        {
            if (clip == null) return;
            
            _musicSource.clip = clip;
            _musicSource.loop = loop;
            _musicSource.Play();
        }

        public void PlayMusic(string id, bool loop = true)
        {
            var clip = FindClip(id);
            if (clip != null)
            {
                PlayMusic(clip, loop);
            }
            else
            {
                Debug.LogWarning($"[AudioManager] Music ID '{id}' not found in any loaded database.");
            }
        }

        public void PlaySFX(AudioClip clip)
        {
            if (clip == null) return;
            _sfxSource.PlayOneShot(clip);
        }

        public void PlaySFX(string id)
        {
            var clip = FindClip(id);
            if (clip != null)
            {
                PlaySFX(clip);
            }
            else
            {
                Debug.LogWarning($"[AudioManager] SFX ID '{id}' not found in any loaded database.");
            }
        }

        public void SetMusicVolume(float volume)
        {
            _musicSource.volume = Mathf.Clamp01(volume);
        }

        public void SetSFXVolume(float volume)
        {
            _sfxSource.volume = Mathf.Clamp01(volume);
        }

        public void Initialize()
        {
            
        }
    }
}
