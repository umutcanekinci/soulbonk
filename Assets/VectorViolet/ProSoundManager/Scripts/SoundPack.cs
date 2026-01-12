using UnityEngine;
using System.Collections.Generic;

namespace VectorViolet.Core.Audio
{
    [CreateAssetMenu(fileName = "NewSoundPack", menuName = "Audio/Sound Pack")]
    public class SoundPack : ScriptableObject
    {
        [Header("Pack Settings")]
        [Tooltip("If true, a random sound will be picked. If false, sounds will play in sequence.")]
        public bool useRandom = true;

        [Tooltip("List of SoundData variations.")]
        public List<SoundData> soundVariations = new List<SoundData>();

        private int _lastIndex = -1;

        /// <summary>
        /// Gets a SoundData from the pack based on the settings.
        /// </summary>
        public SoundData GetSoundData()
        {
            if (soundVariations == null || soundVariations.Count == 0)
            {
                Debug.LogWarning($"[SoundPack] {name} is empty!");
                return null;
            }

            if (soundVariations.Count == 1) return soundVariations[0];

            if (useRandom)
            {
                int randomIndex = Random.Range(0, soundVariations.Count);
                
                // (Optional Improvement) Here you can add a "No Repeat" logic
                // to prevent the same sound from playing consecutively.
                return soundVariations[randomIndex];
            }
            else
            {
                _lastIndex = (_lastIndex + 1) % soundVariations.Count;
                return soundVariations[_lastIndex];
            }
        }
    }
}