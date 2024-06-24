using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using System.Text;
using AudioPlayer;
using DG.Tweening;
using Unity.VisualScripting;

namespace GDC.Managers
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance {get; private set;}
        [SerializeField] GameObject sfxPrefab;        
        [SerializeField] SoundMapConfig soundMapConfig;
        [SerializeField] int numberOfDefaultSFX = 8;

        [Space(10)]
        [SerializeField] float sfxVolume;
        [SerializeField] float musicVolume;

        [Space(10)]
        [SerializeField] AudioMixer currentMixer;
        [SerializeField] AudioMixerGroup SFX_MixerGroup;
        [SerializeField] AudioMixerGroup Music_MixerGroup;

        private bool isAdPauseUserMusic = false;

        private List<AudioSourceController> sfxAudioSrcChannel;
        private Dictionary<SoundID, AudioSource> aloneAudioSourceChannel;
        private Dictionary<SoundID, IEnumerator> aloneAudioSourceFadeCommand;
        private Dictionary<SoundID, int> continuousTimeSampleDict;
        private List<SoundID> m_soundIDPlayedInCurrentFrame;

        private const string MASTER_VOLUME = "MasterVolume";
        private const string MUSIC_VOLUME = "MusicVolume";
        private const string SFX_VOLUME = "SFXVolume";
        private const string AMBIENCE_VOLUME = "AmbienceVolume";

        private bool isDisableMusic = false;

        private Queue<string> _logQueue;
        private StringBuilder _logStringBuilder;

        private List<SoundMap> soundMaps;
        public static System.Action ON_FINISH_LOADING_SOUNDMAP;
        private Coroutine introCoroutine, dialogueCoroutine;
        private List<SoundID> currentMusicIDPlaying;
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            this._logQueue = new Queue<string>();
            this._logStringBuilder = new StringBuilder();

            sfxAudioSrcChannel = new List<AudioSourceController>();
            aloneAudioSourceChannel = new Dictionary<SoundID, AudioSource>();
            aloneAudioSourceFadeCommand = new Dictionary<SoundID, IEnumerator>();
            continuousTimeSampleDict = new Dictionary<SoundID, int>();
            m_soundIDPlayedInCurrentFrame = new List<SoundID>();
            this.enabled = false;

            this.soundMaps = new List<SoundMap>();
            LoadCommonSoundMaps();

            StartCoroutine(Init());

            currentMusicIDPlaying = new List<SoundID>();
        }

        public IEnumerator Init()
        {
            for (int i = 0; i < numberOfDefaultSFX; i++)
            {
                AudioSourceController newSFX = Instantiate(sfxPrefab, transform, false).GetComponent<AudioSourceController>();
                sfxAudioSrcChannel.Add(newSFX);
            }
            yield return null;
            this.enabled = true;
            LoadSave();
            ON_FINISH_LOADING_SOUNDMAP?.Invoke();
        }

        // private void OnAdPauseUserMusic()
        // {
        //     if (!isAdPauseUserMusic)
        //     {
        //         isAdPauseUserMusic = true;
        //         SetMute(isAdPauseUserMusic);
        //     }
        // }

        // private void OnAdResumeUserMusic()
        // {
        //     if (isAdPauseUserMusic)
        //     {
        //         isAdPauseUserMusic = false;
        //         SetMute(isAdPauseUserMusic);
        //     }
        // }

        void Update()
        {
// #if UNITY_EDITOR || UNITY_STANDALONE
//             if( Input.GetKeyDown( KeyCode.M ) )
//             {
//                 // * Mute key cheat
//                 Debug.Log( "M key was pressed." );
//                 TriggerDisableMusic();
//             }
// #endif
        }

        void TriggerDisableMusic()
        {
            isDisableMusic = !isDisableMusic;
            SetMusicMute(isDisableMusic);
        }

        void LateUpdate()
        {
            float dt = Time.unscaledDeltaTime;
            /*if (sfxAudioSrcChannel.Count > 8)
            {
                AudioSource audioSource = sfxAudioSrcChannel.Find(x => !x.isPlaying);

                if (audioSource != null)
                {
                    sfxAudioSrcChannel.Remove(audioSource);
                    Destroy(audioSource.gameObject);
                }
            }*/
            if (Time.frameCount % 10 == 0)
            {
                for (int i = 0; i < sfxAudioSrcChannel.Count; i++)
                {
                    AudioSourceController item = sfxAudioSrcChannel[i];
                    if (!item.audioSource.isPlaying && item.gameObject.activeSelf)
                    {
                        item.gameObject.SetActive(false);
                    }
                }
            }

            m_soundIDPlayedInCurrentFrame.Clear();
        }

        private AudioSource GetFreeSFXChannel(bool requestLowPriority = false)
        {
            AudioSourceController freeAS = sfxAudioSrcChannel.Find(x => !x.gameObject.activeSelf);

            /*if (freeAS == null)
            {
                AudioSource newSFX = Instantiate(sfxPrefab, transform, false).GetComponent<AudioSource>();
                sfxAudioSrcChannel.Add(newSFX);
                return newSFX;
            }*/
            if (freeAS == null)
            {
                if (requestLowPriority)
                {
                    freeAS = sfxAudioSrcChannel.Find(x => x.IsLowPriority);
                    if (freeAS == null)
                    {
                        return null;
                    }
                }
                else
                {
                    int index = Random.Range(0, sfxAudioSrcChannel.Count);
                    freeAS = sfxAudioSrcChannel[index];
                }
            }
            return freeAS.audioSource;
        }

        private AudioSource GetAloneAudioSource(SoundID soundID)
        {
            AudioSource audioSource;
            if (!aloneAudioSourceChannel.TryGetValue(soundID, out audioSource))
            {
                AudioSourceController controller = sfxAudioSrcChannel.Find(x => !x.gameObject.activeSelf);

                if (controller == null)
                    audioSource = Instantiate(sfxPrefab, transform, false).GetComponent<AudioSource>();
                else
                {
                    sfxAudioSrcChannel.Remove(controller);
                    audioSource = controller.audioSource;;
                }
                aloneAudioSourceChannel.Add(soundID, audioSource);
            }
            return audioSource;
        }
        private AudioSource GetNewSFXChannel()
        {
            return Instantiate(sfxPrefab, transform, false).GetComponent<AudioSource>();
        }

        string _logSoundIDFormat = "Frame <color=#00FF30>{0}</color> Play SoundID: <color=#00FF30>{1}</color> - <color=#00FF30>{2}</color>";
        public AudioSource PlaySound(SoundID soundID, float volume = 1f)
        {
            if (soundID == SoundID.NONE) return null;

            SoundMapping soundMapping = null;
            bool foundSound = false;
            foreach (var soundMap in this.soundMaps)
            {
                foreach (var sound in soundMap.SoundMappingList)
                {
                    if (sound.Id == soundID)
                    {
                        soundMapping = sound;
                        foundSound = true;
                        break;
                    }
                }

                if (foundSound)
                    break;
            }
            if (soundMapping == null)
            {
                string errorLog = string.Format("SoundMgr {0} missing mapping !", soundID.ToString());
                Debug.LogError(errorLog);
                this.Log(errorLog);
                return null;
            }

            for (int i = 0; i < m_soundIDPlayedInCurrentFrame.Count; i++)
            {
                if (m_soundIDPlayedInCurrentFrame[i] == soundID)
                {
                    // string warningLog = string.Format("SoundMgr {0} already played in this frame!", soundID.ToString());
                    // Debug.LogWarning(warningLog);
                    return null;
                }
            }
            SoundClipData sfxData;
            sfxData = soundMapping.Data;

            AudioSource sfxAudioSrc;
            if (sfxData.IsAlone || sfxData.IsContinues)
            {
                sfxAudioSrc = GetAloneAudioSource(soundID);
            }
            else
                sfxAudioSrc = GetFreeSFXChannel(sfxData.IsLowPriority);

            if (sfxAudioSrc == null) return null;

            if (soundMapping == null)
            {
                Debug.LogWarning("Can't find data!");
                return null;
            }

            sfxAudioSrc.timeSamples = 0;
            sfxAudioSrc.spatialBlend = 0f;
            sfxAudioSrc.panStereo = 0.0f;
            sfxAudioSrc.volume = volume * GetSFXVolume();
            sfxAudioSrc.loop = sfxData.IsLoop;

            sfxAudioSrc.outputAudioMixerGroup = sfxData.SpecificMixerGroup != null ? sfxData.SpecificMixerGroup : SFX_MixerGroup;

            if (sfxData.IsPitch)
                // sfxAudioSrc.pitch = Random.Range(0.94f, 1.06f);
            {
                float rand = Random.Range(2f,10f);
                sfxAudioSrc.pitch = rand;
            }
            else
                sfxAudioSrc.pitch = 1.0f;

            sfxAudioSrc.clip = sfxData.GetClip();
            sfxAudioSrc.gameObject.SetActive(true);
            sfxAudioSrc.Play();
            m_soundIDPlayedInCurrentFrame.Add(soundID);

            this.Log(string.Format(this._logSoundIDFormat, Time.frameCount ,soundID, sfxAudioSrc.clip.name));

            if ((sfxData.IsAlone || sfxData.IsContinues) && sfxData.FadeInTime > 0f)
                StartCoroutine(Coroutine_FadeInSFX(sfxAudioSrc, sfxData.FadeInTime, null));
            return sfxAudioSrc;
        }

        public AudioSource PlaySoundRelatedToCamera(Vector3 position, SoundID soundID, float volume = 1f)
        {
            float distance = Vector2.Distance(position, Camera.main.transform.position);
            float rate;
            if (distance <= 2f && distance >= 0f) rate = 1;
            else rate = Mathf.Clamp(-2f/15 * distance + 19f/15, 0, 1);
            return PlaySound(soundID, volume * rate);
        }
        
        /// <summary>
        /// Dont worry about this method
        /// </summary>
        public AudioSource PlaySoundHelper(SoundID soundID, float volume = 1f)
        {
            if (soundID == SoundID.NONE) return null;

            SoundMapping soundMapping = null;
            bool foundSound = false;
            foreach (var soundMap in this.soundMaps)
            {
                foreach (var sound in soundMap.SoundMappingList)
                {
                    if (sound.Id == soundID)
                    {
                        soundMapping = sound;
                        foundSound = true;
                        break;
                    }
                }

                if (foundSound)
                    break;
            }
            if (soundMapping == null)
            {
                string errorLog = string.Format("SoundMgr {0} missing mapping !", soundID.ToString());
                Debug.LogError(errorLog);
                this.Log(errorLog);
                return null;
            }

            for (int i = 0; i < m_soundIDPlayedInCurrentFrame.Count; i++)
            {
                if (m_soundIDPlayedInCurrentFrame[i] == soundID)
                {
                    // string warningLog = string.Format("SoundMgr {0} already played in this frame!", soundID.ToString());
                    // Debug.LogWarning(warningLog);
                    return null;
                }
            }
            SoundClipData sfxData;
            sfxData = soundMapping.Data;

            AudioSource sfxAudioSrc;
            if (sfxData.IsAlone || sfxData.IsContinues)
            {
                sfxAudioSrc = GetAloneAudioSource(soundID);
            }
            else
                sfxAudioSrc = GetFreeSFXChannel(sfxData.IsLowPriority);

            if (sfxAudioSrc == null) return null;

            if (soundMapping == null)
            {
                Debug.LogWarning("Can't find data!");
                return null;
            }

            sfxAudioSrc.timeSamples = 0;
            sfxAudioSrc.spatialBlend = 0f;
            sfxAudioSrc.panStereo = 0.0f;
            sfxAudioSrc.volume = volume;
            sfxAudioSrc.loop = sfxData.IsLoop;

            // sfxAudioSrc.outputAudioMixerGroup = sfxData.SpecificMixerGroup != null ? sfxData.SpecificMixerGroup : SFX_MixerGroup;
            sfxAudioSrc.outputAudioMixerGroup = this.Music_MixerGroup;

            if (sfxData.IsPitch)
                sfxAudioSrc.pitch = Random.Range(0.94f, 1.06f);
            else
                sfxAudioSrc.pitch = 1.0f;

            sfxAudioSrc.clip = sfxData.GetClip();
            sfxAudioSrc.gameObject.SetActive(true);
            sfxAudioSrc.Play();
            m_soundIDPlayedInCurrentFrame.Add(soundID);

            this.Log(string.Format(this._logSoundIDFormat, Time.frameCount ,soundID, sfxAudioSrc.clip.name));

            if ((sfxData.IsAlone || sfxData.IsContinues) && sfxData.FadeInTime > 0f)
                StartCoroutine(Coroutine_FadeInSFX(sfxAudioSrc, sfxData.FadeInTime, null));
            return sfxAudioSrc;
        }

        public AudioSource PlayMusic(SoundID soundID, float volume = 1f, float previousMusicFadeOutTime = 0)
        {
            foreach (var i in currentMusicIDPlaying)
            {
                if (i == soundID)
                {
                    print("Current music is playing");
                    return null;
                }
            }
            StopAllMusicPlaying(previousMusicFadeOutTime);
            currentMusicIDPlaying.Add(soundID);

            AudioSource audioSource = null;
            var go = new GameObject();
            go.transform.DOMove(Vector2.zero, previousMusicFadeOutTime).OnComplete(() => {

                audioSource = this.PlaySoundHelper(soundID, volume * GetMusicVolume());
                audioSource.outputAudioMixerGroup = this.Music_MixerGroup;

                Destroy(go, 0.2f);
            });
            
            return audioSource;
        }

        public void UnPauseSFX(SoundID soundID)
        {
            SoundMapping soundMapping = null;
            bool foundSound = false;
            foreach (var soundMap in this.soundMaps)
            {
                foreach (var sound in soundMap.SoundMappingList)
                {
                    if (sound.Id == soundID)
                    {
                        soundMapping = sound;
                        foundSound = true;
                        break;
                    }
                }
                if (foundSound)
                    break;
            }
            if (soundMapping == null)
            {
                Debug.LogWarning("Can't find data!");
                return;
            }

            SoundClipData sfxData;
            sfxData = soundMapping.Data;

            if (sfxData.IsAlone)
            {
                if (aloneAudioSourceChannel.TryGetValue(soundID, out AudioSource sfxAudioSrc))
                {
                    if (sfxAudioSrc.isPlaying) return;

                    IEnumerator coroutine;
                    if (aloneAudioSourceFadeCommand.TryGetValue(soundID, out coroutine))
                    {
                        aloneAudioSourceFadeCommand.Remove(soundID);
                        StopCoroutine(coroutine);
                    }
                    sfxAudioSrc.UnPause();

                    coroutine = Coroutine_FadeInSFX(sfxAudioSrc, sfxData.FadeInTime, null);
                    StartCoroutine(coroutine);
                }
                else
                    PlaySound(soundID);
            }
            else if (sfxData.IsContinues)
            {
                AudioSource sfxAudioSrc;
                if (aloneAudioSourceChannel.TryGetValue(soundID, out sfxAudioSrc))
                {
                    if (sfxAudioSrc.isPlaying) return;
                }
                sfxAudioSrc = PlaySound(soundID);
                sfxAudioSrc.timeSamples = continuousTimeSampleDict[soundID];
            }
        }

        IEnumerator Coroutine_FadeInSFX(AudioSource aSrc, float time, System.Action action)
        {
            float timeOut = 0f;

            while (timeOut < 1f)
            {
                timeOut += Time.unscaledDeltaTime;
                float volume = timeOut / time;
                if (aSrc == null) yield break;
                aSrc.volume = volume;
                yield return null;
            }

            if (aSrc == null) yield break;
            aSrc.volume = 1f;

            if (action != null) action();
        }

        IEnumerator Coroutine_FadeOutSFX(AudioSource aSrc, float time, System.Action action)
        {
            float timeOut = time;

            while (timeOut > 0f)
            {
                timeOut -= Time.unscaledDeltaTime;
                float volume = timeOut / time;
                aSrc.volume = volume;
                yield return null;
            }

            aSrc.volume = 0f;

            if (action != null) action();
        }

        public void PauseSFX(SoundID soundID)
        {
            SoundMapping soundMapping = null;
            bool foundSound = false;
            foreach (var soundMap in this.soundMaps)
            {
                foreach (var sound in soundMap.SoundMappingList)
                {
                    if (sound.Id == soundID)
                    {
                        soundMapping = sound;
                        foundSound = true;
                        break;
                    }
                }
                if (foundSound)
                    break;
            }
            if (soundMapping == null)
            {
                Debug.LogWarning("Can't find data!");
                return;
            }

            SoundClipData sfxData;
            sfxData = soundMapping.Data;
            if (aloneAudioSourceChannel.TryGetValue(soundID, out AudioSource sfxAudioSrc) && sfxAudioSrc.isPlaying)
            {
                if (sfxData.IsAlone)
                {
                    IEnumerator coroutine;
                    if (aloneAudioSourceFadeCommand.TryGetValue(soundID, out coroutine))
                    {
                        aloneAudioSourceFadeCommand.Remove(soundID);
                        StopCoroutine(coroutine);
                    }

                    coroutine = Coroutine_FadeOutSFX(sfxAudioSrc, sfxData.FadeOutTime, () => sfxAudioSrc.Pause());
                    aloneAudioSourceFadeCommand.Add(soundID, coroutine);
                    StartCoroutine(coroutine);

                    if (introCoroutine != null) StopCoroutine(introCoroutine);
                }

                if (sfxData.IsContinues)
                {
                    continuousTimeSampleDict[soundID] = sfxAudioSrc.timeSamples;

                    aloneAudioSourceChannel.Remove(soundID);
                    sfxAudioSrcChannel.Add(sfxAudioSrc.GetComponent<AudioSourceController>());

                    StartCoroutine(Coroutine_FadeOutSFX(sfxAudioSrc, sfxData.FadeOutTime, () =>
                    {
                        sfxAudioSrc.Stop();
                    }));

                    if (introCoroutine != null) StopCoroutine(introCoroutine);
                }
            }
        }
        
        /// <summary>
        /// Stop SFX immediately, not depends on anything, with fade out.
        /// </summary>
        public void StopSFX(SoundID soundID, float fadeOutTime = 0)
        {
            SoundMapping soundMapping = null;
            bool foundSound = false;
            foreach (var soundMap in this.soundMaps)
            {
                foreach (var sound in soundMap.SoundMappingList)
                {
                    if (sound.Id == soundID)
                    {
                        soundMapping = sound;
                        foundSound = true;
                        break;
                    }
                }
                if (foundSound)
                    break;
            }
            if (soundMapping == null)
            {
                Debug.LogWarning("Can't find data!");
                return;
            }

            SoundClipData sfxData;
            sfxData = soundMapping.Data;


            if (aloneAudioSourceChannel.TryGetValue(soundID, out AudioSource sfxAudioSrc))
            {
                aloneAudioSourceChannel.Remove(soundID);
                sfxAudioSrc.DOFade(0, fadeOutTime).OnComplete(() => {
                    sfxAudioSrc.Stop();
                    Destroy(sfxAudioSrc.gameObject);
                    AudioSourceController newSFX = Instantiate(sfxPrefab, transform, false).GetComponent<AudioSourceController>();
                    sfxAudioSrcChannel.Add(newSFX);
                });
            }

            // if (aloneAudioSourceFadeCommand.TryGetValue(soundID, out IEnumerator coroutine))
            // {
            //     aloneAudioSourceFadeCommand.Remove(soundID);
            //     StopCoroutine(coroutine);
            // }
                    
            foreach (var sfx in sfxAudioSrcChannel)
            {
                if (sfx.gameObject.activeSelf == false) continue;
                if (sfx.audioSource.clip == sfxData.GetClip()) 
                {
                    // sfx.audioSource.Stop();
                    sfx.audioSource.DOFade(0, fadeOutTime).OnComplete(() => {
                        sfx.audioSource.Stop();
                    });
                    return;
                }
            }
        }

        /// <summary>
        /// Stop SFX after a period of time, not depends on anything, with fade out.
        /// </summary>
        public void StopSFXAfterDelay(SoundID soundID, float delayTime, float fadeOutTime = 0)
        {
            StartCoroutine(Cor_StopSFXAfterDelay(soundID, delayTime, fadeOutTime));
        }

        /// <summary>
        /// Stop all music playing, with fade out
        /// </summary>
        public void StopAllMusicPlaying(float fadeOutTime = 0)
        {
            if (introCoroutine != null) {
                StopCoroutine(introCoroutine);
            }
            foreach (var id in currentMusicIDPlaying) {
                StopSFX(id, fadeOutTime);
            }
            currentMusicIDPlaying.Clear();
        }
        IEnumerator Cor_StopSFXAfterDelay(SoundID soundID, float delayTime, float fadeOutTime = 0)
        {
            yield return new WaitForSeconds(delayTime);
            StopSFX(soundID, fadeOutTime);
        }

        void SetVolume(string name, float value)
        {
            float db = 20.0f * (value < 0.001f ? -4.0f : Mathf.Log10(value));
            currentMixer.SetFloat(name, db);
            PlayerPrefs.SetFloat(name, value);
        }

        void SetVolume(string name, float value, bool needSave = true)
        {
            float db = 20.0f * (value < 0.001f ? -4.0f : Mathf.Log10(value));
            currentMixer.SetFloat(name, db);
            if (needSave)
            {
                PlayerPrefs.SetFloat(name, value);
            }
        }

        public void SetSFXVolume(float volumeValue, bool needSave = true)
        {
            sfxVolume = volumeValue;
            SetVolume(SFX_VOLUME, volumeValue, needSave);
            SetVolume(AMBIENCE_VOLUME, volumeValue, needSave);
        }
        public float GetMusicVolume()
        {
            return this.musicVolume;
        }
        public float GetSFXVolume()
        {
            return this.sfxVolume;
        }
        public void SetMusicVolume(float volumeValue, bool needSave = true)
        {
            musicVolume = volumeValue;
            SetVolume(MUSIC_VOLUME, volumeValue, needSave);
        }

        public IEnumerator SetMusicVolumeBack(float delaytime)
        {
            yield return new WaitForSeconds(delaytime);
            SetVolume(MUSIC_VOLUME, 1.0f);
        }

        public void SetMute(bool isMute = true)
        {
            SetVolume(MASTER_VOLUME, isMute ? 0.0f : 1.0f);
        }

        public void SetSFXMute(bool isMute = true)
        {
            sfxVolume = isMute ? 0.0f : 1.0f;
            SetVolume(SFX_VOLUME, sfxVolume);
            SetVolume(AMBIENCE_VOLUME, sfxVolume);
            Save();
        }

        public void SetMusicMute(bool isMute = true)
        {
            musicVolume = isMute ? 0.0f : 1.0f;
            SetVolume(MUSIC_VOLUME, musicVolume);
            Save();
        }

        public bool LoadSave()
        {
            sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME, 1.0f);
            musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME, 1.0f);

            SetVolume(MUSIC_VOLUME, musicVolume);
            SetVolume(SFX_VOLUME, sfxVolume);
            SetVolume(AMBIENCE_VOLUME, sfxVolume);
            
            return true;
        }
        void Save()
        {
            PlayerPrefs.SetFloat(SFX_VOLUME, sfxVolume);
            PlayerPrefs.SetFloat(MUSIC_VOLUME, musicVolume);
            PlayerPrefs.Save();
        }

        public string LOG_TAG = "SoundMgr";
        void Log(string log)
        {
            if(this._logQueue.Count > 20)
                this._logQueue.Dequeue();

            this._logQueue.Enqueue(log);

            this._logStringBuilder.Clear();
            foreach(string item in this._logQueue)
            {
                this._logStringBuilder.AppendLine(item);
            }
        }

        public string GetDebugLog()
        {
            return this._logStringBuilder.ToString();
        }

        private void LoadCommonSoundMaps()
        {
            LoadSoundMap(SoundType.MAIN_MENU);
            LoadSoundMap(SoundType.COMMON);
        }

        public void LoadSoundMap(SoundType soundType)
        {
            for (int i = 0; i < this.soundMaps.Count; i++)
            {
                if (this.soundMaps[i].soundType == soundType)
                    return; // Already loaded
            }

            for (int i = 0; i < soundMapConfig.MapPath.Count; i++)
            {
                if (soundMapConfig.MapPath[i].Type == soundType)
                {
                    SoundMap soundMap = Resources.Load<SoundMap>(soundMapConfig.MapPath[i].Path);
                    soundMap.LoadSoundData();
                    this.soundMaps.Add(soundMap);
                    return;
                }
            }
        }

        public void UnloadSoundMap(SoundType soundType)
        {
            for (int i = 0; i < this.soundMaps.Count; i++)
            {
                if (this.soundMaps[i].soundType == soundType)
                {
                    Resources.UnloadAsset(this.soundMaps[i]);
                    this.soundMaps.RemoveAt(i);
                    //Resources.UnloadUnusedAssets();
                    return;
                }
            }
        }
        /// <summary>
        /// All soundmaps will be deleted, except COMMON SoundMap
        /// </summary>
        public void ClearSoundMapExceptCommonSoundMap()
        {
            foreach (var cur in currentMusicIDPlaying)
            {
                var temp = GetSoundClipData(cur);
                if (temp == null) break;
                for (int i = 0; i < sfxAudioSrcChannel.Count; i++)
                {
                    AudioSourceController item = sfxAudioSrcChannel[i];
                    if (item.gameObject.activeSelf && item.audioSource.clip == temp.GetClip())
                        item.audioSource.DOFade(0, 1.5f).OnComplete(() => item.gameObject.SetActive(false));
                }
            }
            for (int i = 0; i < soundMaps.Count; i++)
            {
                if (soundMaps[i].soundType == SoundType.COMMON) continue;
                Resources.UnloadAsset(soundMaps[i]);
                soundMaps.RemoveAt(i);
            }
        }
        /// <summary>
        /// All soundmaps will be deleted
        /// </summary>
        public void ClearSoundMap()
        {
            foreach (var cur in currentMusicIDPlaying)
            {
                var temp = GetSoundClipData(cur);
                if (temp == null) break;
                for (int i = 0; i < sfxAudioSrcChannel.Count; i++)
                {
                    AudioSourceController item = sfxAudioSrcChannel[i];
                    if (item.gameObject.activeSelf && item.audioSource.clip == temp.GetClip())
                        item.audioSource.DOFade(0, 1.5f).OnComplete(() => item.gameObject.SetActive(false));
                }
            }

            for (int i = 0; i < this.soundMaps.Count; i++)
            {
                Resources.UnloadAsset(this.soundMaps[i]);
                this.soundMaps.RemoveAt(i);
            }
        }

        public SoundClipData GetSoundClipData(SoundID soundID)
        {
            if (soundID == SoundID.NONE) return null;

            foreach (var soundMap in this.soundMaps)
            {
                foreach (var sound in soundMap.SoundMappingList)
                {
                    if (sound.Id == soundID)
                    {
                        return sound.Data;
                    }
                }
            }

            return null;
        }
        public void PlayMusicWithIntro(SoundID introMusic, SoundID music, float volume = 1f, float previousMusicFadeOutTime = 0)
        {
            foreach (var soundID in currentMusicIDPlaying)
            {
                if (soundID == introMusic || soundID == music) 
                {
                    print("Current music is playing");
                    return;
                }
            }
            introCoroutine = StartCoroutine(Cor_WaitEndIntro(introMusic, music, volume, previousMusicFadeOutTime));
        }
        IEnumerator Cor_WaitEndIntro(SoundID introMusic, SoundID music, float volume, float previousMusicFadeOutTime = 0f)
        {
            var soundClipData = GetSoundClipData(introMusic);

            var audioSource = PlayMusic(introMusic, volume, previousMusicFadeOutTime);
            // audioSource.loop = false;
            yield return new WaitForSecondsRealtime(soundClipData.GetClip().length + previousMusicFadeOutTime);
            StopSFX(introMusic);
            audioSource = PlayMusicWithoutClear(music, volume);
            aloneAudioSourceChannel.Remove(introMusic);
            // audioSource.loop = true;
        }
        AudioSource PlayMusicWithoutClear(SoundID soundID, float volume = 1f)
        {
            currentMusicIDPlaying.Add(soundID);
            var audioSource = PlaySound(soundID, volume);
            audioSource.outputAudioMixerGroup = Music_MixerGroup;

            return audioSource;
        }
    
        public void PlaySoundDialogue(SoundID soundID, int frequency)
        {
            dialogueCoroutine = StartCoroutine(Cor_PlaySoundWithFrequency(soundID, frequency));
        }
        IEnumerator Cor_PlaySoundWithFrequency(SoundID soundID, int frequency)
        {
            while (true)
            {
                for (int i=0; i<frequency; i++)
                {
                    PlaySound(soundID);
                    yield return new WaitForSeconds(1f/frequency);
                }
            }
        }
        public void StopSoundDialogue()
        {
            StopCoroutine(dialogueCoroutine);
        }
    }
}