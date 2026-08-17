using System;
using UnityEngine;
using UnityEngine.Audio;
using Workshop.Scaffolding.Nature.Scripts.Collectible;

namespace Workshop.Scaffolding.Nature.Scripts.Audio.Manager
{
    public class UnityAudioManager : AudioManager
    {
        [Header("Audio Mixer")]
        public AudioMixer audioMixer;

        [Header("Footsteps")]
        public AudioSource footstepSource;
        public AudioClip[] footstepDirtClips;
        public AudioClip[] footstepStoneClips;
        public AudioClip[] footstepWoodClips;

        [Header("Ambience")]
        public AudioSource daySource;
        public AudioSource nightSource;
        public AudioClip dayAmbienceClip;
        public AudioClip nightAmbienceClip;

        [Header("Music")]
        public AudioSource musicSource;
        public AudioClip musicClip;

        [Header("Collectible")]
        public GameObject collectibleSource;
        

        private void OnEnable()
        {
            fpsController.OnFootstepDetected += HandleFootstepDetected;
            dayNightCycleController.OnDayNightCycleValueChanged += HandleDayNightCycleValueChanged;
            CollectibleTracker.Instance.OnCollectibleGathered += HandleCollectibleGathered;
            // CollectibleTracker = Singleton => se acceseaza cu .Instance

            audioOptionsUIController.OnAudioOptionChanged += HandleAudioOptionChanged;
        }

        private void OnDisable()
        {
            fpsController.OnFootstepDetected -= HandleFootstepDetected;
            dayNightCycleController.OnDayNightCycleValueChanged -= HandleDayNightCycleValueChanged;
            // OBS: daca am folosi .Instance == null, ar fi true cand se opreste scena
            // (deoarece instanta e stearsa automat) => obiectul Singleton si-ar crea o instanta noua (prin .get())
            // => ar ramane un obiect fantoma => eroare
            if (CollectibleTracker.HasInstance)
            {
                CollectibleTracker.Instance.OnCollectibleGathered -= HandleCollectibleGathered;
            }
        }

        private void Start()    // folosim Start() ci nu Awake() pt a fi siguri ca totul e pregatit si AudioSource-urile folosite exista (daca erau configurate intr-o metoda Awake() altundeva ar fi fost important)
        {
            daySource.volume = 1f;
            daySource.clip = dayAmbienceClip;
            daySource.Play();                   // Play() permite loop (setat din editor sau prin cod)
                                                // PlayOneShot() nu permite
            nightSource.volume = 0f;
            nightSource.clip = nightAmbienceClip;
            nightSource.Play();

            musicSource.volume = 0.5f;
            musicSource.clip = musicClip;
            musicSource.Play();
        }

        private void HandleFootstepDetected(AudioUtils.AudioSurfaceType type, float speed)
        {
            Debug.Log($"Footstep detected on {type} surface at speed {speed}");
            
            int idx = 0;

            switch (type)
            {
                case AudioUtils.AudioSurfaceType.Dirt:
                    idx = UnityEngine.Random.Range(0, footstepDirtClips.Length);
                    //Debug.Log($"Playing {idx}");
                    footstepSource.PlayOneShot(footstepDirtClips[idx]);
                    break;

                case AudioUtils.AudioSurfaceType.Stone:
                    idx = UnityEngine.Random.Range(0, footstepStoneClips.Length);
                    //Debug.Log($"Playing {idx}");
                    footstepSource.PlayOneShot(footstepStoneClips[idx]);
                    break;

                case AudioUtils.AudioSurfaceType.Wood:
                    idx = UnityEngine.Random.Range(0, footstepWoodClips.Length);
                    //Debug.Log($"Playing {idx}");
                    footstepSource.PlayOneShot(footstepWoodClips[idx]);
                    break;

                case AudioUtils.AudioSurfaceType.None:break;
                default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private void HandleDayNightCycleValueChanged(float sliderValue)
        {
            Debug.Log($"Day-night cycle value change detected at {sliderValue}");

            daySource.volume = 1f - sliderValue;
            nightSource.volume = sliderValue;
        }

        private void HandleCollectibleGathered(CollectibleData data)
        {
            if (data.Clip == null || data.Position == null || collectibleSource == null) return;

            // Instantiam un obiect prefab cu un AudioSource la pozitia primita, fara rotatie
            GameObject audioInstance = Instantiate(collectibleSource, data.Position, Quaternion.identity);
            // Retinem componenta AudioSource pentru a-i seta Clip-ul si a-i da Play() din linie de cod
            AudioSource source = audioInstance.GetComponent<AudioSource>();

            source.clip = data.Clip;
            source.volume = 1f;
            source.Play();

            // Distrugem obiectul dupa ce se termina Clip-ul
            Destroy(audioInstance, data.Clip.length);
        }

        private void HandleAudioOptionChanged(AudioUtils.AudioOptionType type, float value)
        {
            value = Mathf.Clamp(value, 0.0001f, 1f);
            var dbValue = Mathf.Log10(value) * 20;

            switch (type)
            {
                case AudioUtils.AudioOptionType.Master: audioMixer.SetFloat("MasterVolume", dbValue); break;
                case AudioUtils.AudioOptionType.SFX: audioMixer.SetFloat("SFXVolume", dbValue); break;
                case AudioUtils.AudioOptionType.Ambience: audioMixer.SetFloat("AmbienceVolume", dbValue); break;
                case AudioUtils.AudioOptionType.Music: audioMixer.SetFloat("MusicVolume", dbValue); break;
                default: throw new ArgumentOutOfRangeException(nameof(type), type, null);

            }
        }
    }
}
