using System.Collections.Generic;
using UnityEngine;

namespace BubblePops
{
    public class AudioManager : MonoBehaviour
    {
        private static AudioDataSO[] _audioData;

        private static Dictionary<Enums.AudioType, float> audioTimerDictionary;
        private static GameObject oneShotGameObject;
        private static AudioSource oneShotAudioSource;

        private void Awake()
        {
            _audioData = Resources.LoadAll<AudioDataSO>("_AudioData/");
        }

        // we initialized dictionary for delayed sound.
        // Initalize this in Awake which script you want to use delayed sound.
        public static void Initalize()
        {
            audioTimerDictionary = new Dictionary<Enums.AudioType, float>();
            audioTimerDictionary[Enums.AudioType.None] = 0;
        }

        public static void PlayAudio(Enums.AudioType audioType, Vector3 position)
        {
            if (!SettingsManager.SoundOn) return;

            if (CanPlayAudio(audioType))
            {
                GameObject audioGameObject = new GameObject("Audio");
                audioGameObject.transform.position = position;

                AudioSource audioSource = audioGameObject.AddComponent<AudioSource>();
                audioSource.clip = GetAudioClip(audioType);
                audioSource.maxDistance = 100f;
                audioSource.spatialBlend = 1f;
                audioSource.rolloffMode = AudioRolloffMode.Linear;
                audioSource.dopplerLevel = 0f;
                audioSource.Play();

                Object.Destroy(audioGameObject, audioSource.clip.length);// Destroy when clip is finished
            }
        }

        public static void PlayAudio(Enums.AudioType audioType, float volume = 1f, float pitch = 1f)
        {
            if (!SettingsManager.SoundOn) return;

            if (CanPlayAudio(audioType))
            {
                if (oneShotGameObject == null)
                {
                    oneShotGameObject = new GameObject("One Shot Audio");
                    oneShotAudioSource = oneShotGameObject.AddComponent<AudioSource>();
                }

                oneShotAudioSource.loop = false;
                oneShotAudioSource.volume = volume;
                oneShotAudioSource.pitch = pitch;
                oneShotAudioSource.PlayOneShot(GetAudioClip(audioType));
            }
        }

        public static void PlayAudioLoop(Enums.AudioType audioType, float volume = 1f, float pitch = 1f)
        {
            if (!SettingsManager.SoundOn) return;

            if (CanPlayAudio(audioType))
            {
                if (oneShotGameObject == null)
                {
                    oneShotGameObject = new GameObject("One Shot Audio");
                    oneShotAudioSource = oneShotGameObject.AddComponent<AudioSource>();
                }

                oneShotAudioSource.loop = true;
                oneShotAudioSource.volume = volume;
                oneShotAudioSource.pitch = pitch;
                oneShotAudioSource.PlayOneShot(GetAudioClip(audioType));
            }
        }
        public static void StopAudioLoop()
        {
            if (oneShotAudioSource != null && oneShotAudioSource.isPlaying) oneShotAudioSource.Stop();
        }
        public static bool IsAudioPlaying() => oneShotAudioSource != null && oneShotAudioSource.isPlaying;

        // Add delayed audios here.
        private static bool CanPlayAudio(Enums.AudioType audioType)
        {
            switch (audioType)
            {
                default:
                    return true;
                case Enums.AudioType.None:
                    if (audioTimerDictionary.ContainsKey(audioType))
                    {
                        float lastTimePlayed = audioTimerDictionary[audioType];
                        float playerMoveTimerMax = .37f;
                        if (lastTimePlayed + playerMoveTimerMax < Time.time)
                        {
                            audioTimerDictionary[audioType] = Time.time;
                            return true;
                        }
                        else return false;
                    }
                    else return true;
                    //break;
            }
        }

        private static AudioClip GetAudioClip(Enums.AudioType audioType)
        {
            AudioClip clip = null;
            foreach (AudioDataSO audioData in _audioData)
            {
                if (audioData.Type == audioType)
                {
                    if (audioData.Clips.Length > 1)
                        clip = audioData.Clips[Random.Range(0, audioData.Clips.Length)];
                    else
                        clip = audioData.Clips[0];
                }
            }

            if (clip == null)
                Debug.LogWarning($"Audio {audioType} not found!");

            return clip;
        }
    }
}
