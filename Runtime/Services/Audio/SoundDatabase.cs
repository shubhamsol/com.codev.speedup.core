using System;
using System.Collections.Generic;
using UnityEngine;

namespace Speedup.Services.Audio
{
    [Serializable]
    public struct AudioEntry
    {
        [Tooltip("The ID used to play this sound (e.g., 'ui_click', 'bgm_menu')")]
        public string id;
        public AudioClip clip;
    }

    [CreateAssetMenu(fileName = "NewSoundDatabase", menuName = "Audio/Sound Database")]
    public class SoundDatabase : ScriptableObject
    {
        [Tooltip("List of audio clips and their corresponding string IDs")]
        public List<AudioEntry> entries = new List<AudioEntry>();

        /// <summary>
        /// Retrieves an AudioClip by its ID. Returns null if not found.
        /// </summary>
        public AudioClip GetClip(string id)
        {
            foreach (var entry in entries)
            {
                if (entry.id == id)
                {
                    return entry.clip;
                }
            }
            return null;
        }
    }
}
