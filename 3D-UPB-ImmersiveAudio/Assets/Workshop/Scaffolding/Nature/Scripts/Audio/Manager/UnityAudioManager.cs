using System;
using UnityEngine;

namespace Workshop.Scaffolding.Nature.Scripts.Audio.Manager
{
    public class UnityAudioManager : AudioManager
    {
        public AudioSource footstepSource;
        public AudioClip[] footstepDirtClips;
        public AudioClip[] footstepStoneClips;
        public AudioClip[] footstepWoodClips;


        private void OnEnable()
        {
            fpsController.OnFootstepDetected += HandleFootstepDetected;
        }

        private void OnDisable()
        {
            fpsController.OnFootstepDetected -= HandleFootstepDetected;
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
    }
}
