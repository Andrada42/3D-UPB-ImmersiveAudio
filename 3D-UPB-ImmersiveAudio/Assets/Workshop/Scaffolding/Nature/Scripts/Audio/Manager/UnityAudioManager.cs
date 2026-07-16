using System;
using UnityEngine;

namespace Workshop.Scaffolding.Nature.Scripts.Audio.Manager
{
    public class UnityAudioManager : AudioManager
    {
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

        private void OnEnable()
        {
            fpsController.OnFootstepDetected += HandleFootstepDetected;
            dayNightCycleController.OnDayNightCycleValueChanged += HandleDayNightCycleValueChanged;
        }

        private void OnDisable()
        {
            fpsController.OnFootstepDetected -= HandleFootstepDetected;
            dayNightCycleController.OnDayNightCycleValueChanged -= HandleDayNightCycleValueChanged;
        }

        private void Start()    // folosim Start() ci nu Awake() pt a fi siguri ca totul e pregatit si AudioSource-urile folosite exista (daca erau configurate intr-o metoda Awake() altundeva ar fi fost important)
        {
            nightSource.volume = 1f;
            daySource.clip = dayAmbienceClip;
            daySource.Play();                   // Play() permite loop (setat din editor sau prin cod)
                                                // PlayOneShot() nu permite
            nightSource.volume = 0f;
            nightSource.clip = nightAmbienceClip;
            nightSource.Play();
        }

        private void HandleFootstepDetected(AudioUtils.AudioSurfaceType type, float speed)
        {
            Debug.Log($"Footstep detected on {type} surface at speed {speed}");
            
            int idx = 0;

            switch (type)
            {
                case AudioUtils.AudioSurfaceType.Dirt:
                    idx = UnityEngine.Random.Range(0, footstepDirtClips.Length - 1);
                    //Debug.Log($"Playing {idx}");
                    footstepSource.PlayOneShot(footstepDirtClips[idx]);
                    break;

                case AudioUtils.AudioSurfaceType.Stone:
                    idx = UnityEngine.Random.Range(0, footstepStoneClips.Length - 1);
                    //Debug.Log($"Playing {idx}");
                    footstepSource.PlayOneShot(footstepStoneClips[idx]);
                    break;

                case AudioUtils.AudioSurfaceType.Wood:
                    idx = UnityEngine.Random.Range(0, footstepWoodClips.Length - 1);
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
    }
}
