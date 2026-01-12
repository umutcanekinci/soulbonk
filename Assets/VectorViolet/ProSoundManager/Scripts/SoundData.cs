using UnityEngine;

namespace VectorViolet.Core.Audio
{
    [CreateAssetMenu(fileName = "NewSoundData", menuName = "Audio/Sound Data")]
    public class SoundData : ScriptableObject
    {
        [Header("General Settings")]
        [Tooltip("The audio clip to be played.")]
        public AudioClip clip;
        
        [Range(0f, 1f)] 
        [Tooltip("The volume of the audio clip.")]
        public float volume = 1f;
        
        [Tooltip("The pitch of the audio clip.")]
        [Range(0.1f, 3f)] 
        public float pitch = 1f;

        [Tooltip("Should the audio clip loop when played?")]
        public bool loop;

        [Header("Randomization")]
        [Tooltip("Enable to apply random pitch variation.")]
        public bool useRandomPitch = false;
        
        [Range(0f, 1f)]
        [Tooltip("The range of random pitch variation applied to the base pitch.")]
        public float randomPitchRange = 0.1f;
        
        [Header("3D Settings")]
        [Range(0f, 1f)]
        [Tooltip("The spatial blend of the audio clip.")]
        public float spatialBlend = 0f; // 0 = 2D, 1 = 3D. Default is 2D.
        
        [Tooltip("Determines how the sound attenuates over distance.")]
        public AudioRolloffMode rolloffMode = AudioRolloffMode.Linear;

        [Tooltip("The distance where the sound is at its loudest.")]
        public float minDistance = 1f; 
        
        [Tooltip("The distance where the sound becomes inaudible.")]
        public float maxDistance = 50f;

    }
}