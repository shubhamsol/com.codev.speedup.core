using UnityEngine;
using Speedup.Core;

namespace Speedup.Services.Audio
{
    public interface IAudioService : IGameService
    {
        void PlayMusic(AudioClip clip, bool loop = true);
        void PlayMusic(string id, bool loop = true);
        void PlaySFX(AudioClip clip);
        void PlaySFX(string id);
        void SetMusicVolume(float volume);
        void SetSFXVolume(float volume);
        void PlayHaptic(long milliseconds = -1);
        void ToggleHaptics(bool isOn);
        bool HapticsEnabled { get; }
    }
}
