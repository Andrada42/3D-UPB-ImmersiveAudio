using System;
using UnityEngine;

namespace Workshop.Scaffolding.Nature.Scripts.Audio.Manager
{
    public class UnityAudioManager : AudioManager
    {
        public AudioSource footstepSource;
        public AudioClip footstepDirtSound;
        public AudioClip footstepStoneSound;
        public AudioClip footstepWoodSound;


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
            switch (type)
            {
                case AudioUtils.AudioSurfaceType.Dirt: footstepSource.PlayOneShot(footstepDirtSound); break;
                case AudioUtils.AudioSurfaceType.Stone: footstepSource.PlayOneShot(footstepStoneSound); break;
                case AudioUtils.AudioSurfaceType.Wood: footstepSource.PlayOneShot(footstepWoodSound); break;
                case AudioUtils.AudioSurfaceType.None: break;
                default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
