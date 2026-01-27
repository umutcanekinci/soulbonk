using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;
using System.Collections;
using VectorViolet.Core.Utilities;

namespace VectorViolet.Core.Audio
{
    public class SoundManager : Singleton<SoundManager>
    {
        #region 1. Variables
        public List<SoundData> SoundDataList => sfxDataList;
        public List<SoundData> MusicDataList => musicDataList;


        [Header("Sound Data Lists")]
        [Tooltip("List of sound effects data")]
        [SerializeField] private List<SoundData> sfxDataList = new List<SoundData>();

        [Tooltip("List of music data")]
        [SerializeField] private List<SoundData> musicDataList = new List<SoundData>();


        [Header("Audio Mixer")]

        [Tooltip("Main Audio Mixer controlling overall audio levels")]
        [SerializeField] private AudioMixer _mainMixer;

        [Tooltip("Audio Mixer Group for Music playback")]
        [SerializeField] private AudioMixerGroup _musicGroup;

        [Tooltip("Audio Mixer Group for SFX playback")]
        [SerializeField] private AudioMixerGroup _sfxGroup;


        [Header("Pool Settings")]
        [Tooltip("Initial size of the AudioSource pool for SFX playback")]
        [SerializeField] private int _poolSize = 10; 
        
        [Tooltip("If true, the pool can grow dynamically when all sources are in use")]
        [SerializeField] private bool _expandable = true; 


        private Dictionary<string, SoundData> _soundDataDict = new Dictionary<string, SoundData>();
        private AudioSource _musicSource; 
        private Coroutine _musicCoroutine;
        private List<AudioSource> _sfxPool; 
        private GameObject _sfxParent;

        #endregion

        #region 2. Unity Lifecycle & Initialization

        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }

        private void Initialize()
        {
            InitializeSoundDataDictionary(); // Improve lookup efficiency
            InitializeMusicSource();
            InitializePool();
        }

        private void InitializeSoundDataDictionary()
        {
            foreach (var soundData in sfxDataList)
            {
                if (soundData != null && !_soundDataDict.ContainsKey(soundData.clip.name))
                {
                    _soundDataDict.Add(soundData.clip.name, soundData);
                }
            }

            foreach (var musicData in musicDataList)
            {
                if (musicData != null && !_soundDataDict.ContainsKey(musicData.clip.name))
                {
                    _soundDataDict.Add(musicData.clip.name, musicData);
                }
            }
        }

        private void InitializeMusicSource()
        {
            GameObject musicObj = new GameObject("Music_Source");
            musicObj.transform.SetParent(this.transform);
            
            _musicSource = musicObj.AddComponent<AudioSource>();
            _musicSource.outputAudioMixerGroup = _musicGroup;
            _musicSource.loop = true;
            _musicSource.playOnAwake = false;
            _musicSource.spatialBlend = 0f; 
        }

        private void InitializePool()
        {
            _sfxPool = new List<AudioSource>();
            _sfxParent = new GameObject("SFX_Pool_Container");
            _sfxParent.transform.SetParent(this.transform);

            for (int i = 0; i < _poolSize; i++)
            {
                CreateNewSource();
            }
        }

        private AudioSource CreateNewSource()
        {
            GameObject newObj = new GameObject("Pooled_AudioSource");
            newObj.transform.SetParent(_sfxParent.transform);
            
            AudioSource source = newObj.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.outputAudioMixerGroup = _sfxGroup;

            _sfxPool.Add(source);
            return source;
        }

        #endregion

        #region 3. Music Methods

        public void PlayMusic(string clipName)
        {
            if (TryGetSoundData(clipName, out SoundData data))
            {
                PlayMusicInternal(data);
            }
        }

        /// <summary>
        /// Plays the given music immediately.
        /// </summary>
        public void PlayMusic(SoundData data)
        {
            PlayMusicInternal(data);
        }

        public void PlayMusic(string clipName, float delay)
        {
            if (TryGetSoundData(clipName, out SoundData data))
            {
                PlayMusic(data, delay);
            }
        }

        /// <summary>
        /// Plays the given music after a delay in seconds.
        /// </summary>
        public void PlayMusic(SoundData data, float delay)
        {
            if (_musicCoroutine != null)
                StopCoroutine(_musicCoroutine);

            if (delay != 0f)
            {
                _musicCoroutine = StartCoroutine(PlayMusicWithDelay(data, delay));
            }
            else
            {

                PlayMusicInternal(data);
            }
        }

        public void StopMusic()
        {
            if (_musicSource.isPlaying)
            {
                _musicSource.Stop();
            }
        }

        /// <summary>
        /// Adjusts the Music volume using the Audio Mixer.
        /// Input value should be between 0 (mute) and 1 (max).
        /// </summary>
        public void SetMusicVolume(float sliderValue)
        {
            SetVolume(sliderValue, "MusicVol");
        }

        private IEnumerator PlayMusicWithDelay(SoundData data, float delay)
        {
            yield return new WaitForSeconds(delay);
            PlayMusicInternal(data);
        }

        private void PlayMusicInternal(SoundData data)
        {
            if (_musicSource.clip == data.clip && _musicSource.isPlaying) return;

            _musicSource.clip = data.clip;
            _musicSource.volume = data.volume;
            _musicSource.Play();
        }

        #endregion

        #region 4. SFX Methods

        /* Method Chaining
         * 1. Simple Calls Complex
         * 2. Complex Calls Simple
         * 3. Internal Method to avoid code duplication (Selected this one)
        */
        public void PlaySFX(string clipName)
        {
            if (TryGetSoundData(clipName, out SoundData data))
            {
                PlaySFX(data);
            }
        }

        public void PlaySFX(SoundData data)
        {
            PlaySFXInternal(data, Vector3.zero, false);
        }

        public void PlaySFX(string clipName, Vector3 position)
        {
            if (TryGetSoundData(clipName, out SoundData data))
            {
                PlaySFX(data, position);
            }
        }
        
        public void PlaySFX(SoundData data, Vector3 position)
        {
            PlaySFXInternal(data, position, true);
        }

        public void PlaySFX(string clipName, float delay)
        {
            if (TryGetSoundData(clipName, out SoundData data))
            {
                PlaySFX(data, delay);
            }
        }

        public void PlaySFX(SoundData data, float delay)
        {
            if (delay != 0f)
            {
                StartCoroutine(PlaySFXWithDelay(data, Vector3.zero, delay, false));
            }
            else
            {
                PlaySFXInternal(data, Vector3.zero, false);
            }
        }

        public void PlaySFX(string clipName, Vector3 position, float delay)
        {
            if (TryGetSoundData(clipName, out SoundData data))
            {
                PlaySFX(data, position, delay);
            }
        }

        public void PlaySFX(SoundData data, Vector3 position, float delay)
        {
            if (delay != 0f)
            {
                StartCoroutine(PlaySFXWithDelay(data, position, delay, true));
            }
            else
            {
                PlaySFXInternal(data, position, true);
            }
        }

public void PlaySFX(SoundPack pack)
        {
            if (pack == null) return;
            PlaySFXInternal(pack.GetSoundData(), Vector3.zero, false);
        }

        public void PlaySFX(SoundPack pack, Vector3 position)
        {
            if (pack == null) return;
            PlaySFXInternal(pack.GetSoundData(), position, true);
        }

        public void PlaySFX(SoundPack pack, float delay)
        {
            if (pack == null) return;
            SoundData selectedData = pack.GetSoundData();
            
            if (delay != 0f)
            {
                StartCoroutine(PlaySFXWithDelay(selectedData, Vector3.zero, delay, false));
            }
            else
            {
                PlaySFXInternal(selectedData, Vector3.zero, false);
            }
        }

        public void PlaySFX(SoundPack pack, Vector3 position, float delay)
        {
            if (pack == null) return;
            SoundData selectedData = pack.GetSoundData();

            if (delay != 0f)
            {
                StartCoroutine(PlaySFXWithDelay(selectedData, position, delay, true));
            }
            else
            {
                PlaySFXInternal(selectedData, position, true);
            }
        }

        public void StopSFX(string clipName)
        {
            if (TryGetSoundData(clipName, out SoundData data))
            {
                StopSFX(data);
            }
        }

        public void StopSFX(SoundData data)
        {
            foreach (var source in _sfxPool)
            {
                if (source.clip == data.clip && source.isPlaying)
                {
                    source.Stop();
                }
            }
        }

        public void StopAllSFX()
        {
            foreach (var source in _sfxPool)
            {
                if (source.isPlaying)
                {
                    source.Stop();
                }
            }
        }

        public void SetSFXVolume(float sliderValue)
        {
            SetVolume(sliderValue, "SFXVol");
        }

        private AudioSource GetAvailableSource()
        {
            for (int i = 0; i < _sfxPool.Count; i++)
            {
                if (!_sfxPool[i].isPlaying)
                {
                    return _sfxPool[i];
                }
            }

            if (_expandable)
            {
                return CreateNewSource();
            }

            return null;
        }

        private IEnumerator PlaySFXWithDelay(SoundData data, Vector3 position, float delay, bool is3D = false)
        {
            yield return new WaitForSeconds(delay);
            PlaySFXInternal(data, position, is3D);
        }

        private void PlaySFXInternal(SoundData data, Vector3 position, bool is3D)
        {
            AudioSource source = GetAvailableSource();
            if (source == null)
            {
                Debug.LogWarning("No available AudioSource in the pool, and pool expansion is disabled.");
                return;
            }

            // Reset transform if 3D
            if (is3D)
            {
                source.transform.position = position;
                source.spatialBlend = data.spatialBlend;
                source.rolloffMode = data.rolloffMode;
                source.minDistance = data.minDistance;
                source.maxDistance = data.maxDistance;
            }
            else
            {
                source.spatialBlend = 0f; // Force 2D
            }

            // Common settings
            source.clip = data.clip;
            source.volume = data.volume;
            source.pitch = data.pitch;
            source.loop = data.loop;

            if (data.useRandomPitch)
            {
                float randomVariance = Random.Range(-data.randomPitchRange, data.randomPitchRange);
                source.pitch += randomVariance;
            }
            
            source.Play();
        }

        #endregion

        #region 5. Helper Methods
        
        private void SetVolume(float sliderValue, string name)
        {
            _mainMixer.SetFloat(name, SliderValueToDecibels(sliderValue));
        }

        private float SliderValueToDecibels(float value)
        {
            return (value <= 0.001f) ? -80f : Mathf.Log10(value) * 20;
        }

        private bool TryGetSoundData(string clipName, out SoundData data)
        {
            if (_soundDataDict.TryGetValue(clipName, out data))
            {
                return true;
            }
            
            Debug.LogWarning($"[SoundManager] SoundData with clip name '{clipName}' not found.");
            data = null;
            return false;
        }

        #endregion

    }
}