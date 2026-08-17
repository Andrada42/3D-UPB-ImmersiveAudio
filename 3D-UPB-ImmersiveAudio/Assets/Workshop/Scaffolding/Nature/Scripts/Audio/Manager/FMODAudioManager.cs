#if FMOD_INSTALLED

using FMOD.Studio;
using FMODUnity;
using NaughtyAttributes;
using System;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Workshop.Scaffolding.Nature.Scripts.Audio.Manager
{
    public class FMODAudioManager : AudioManager
    {
        [SerializeField, BoxGroup("FMOD Events")]
        private EventReference footstepEvent;

        [SerializeField, BoxGroup("FMOD Events")]
        private EventReference ambientEvent;

        [SerializeField, BoxGroup("FMOD Events")]
        private EventReference jumpEvent;

        [SerializeField, BoxGroup("FMOD Events")]
        private EventReference collectiblePickupEvent;
        
        [SerializeField, BoxGroup("FMOD Events")]
        private EventReference musicEvent;
        
        [SerializeField, BoxGroup("FMOD Events")]
        private EventReference underwaterSnapshot;
        
        [SerializeField, BoxGroup("FMOD VCAs")]
        private string vcaMaster   = "vca:/VCA_Master";
        
        [SerializeField, BoxGroup("FMOD VCAs")]
        private string vcaSFX      = "vca:/VCA_SFX";
        
        [SerializeField, BoxGroup("FMOD VCAs")]
        private string vcaAmbience = "vca:/VCA_Ambience";
        
        [SerializeField, BoxGroup("FMOD VCAs")]
        private string vcaMusic    = "vca:/VCA_Music";

        private EventInstance ambientEventInstance;


        private void OnEnable()
        {
            fpsController.OnFootstepDetected += HandleFootstepDetected;
            dayNightCycleController.OnDayNightCycleValueChanged += HandleDayNightCycleValueChanged;

            fpsController.OnJump += HandleJump;
        
        }

        private void OnDisable()
        {
            fpsController.OnFootstepDetected -= HandleFootstepDetected;
            dayNightCycleController.OnDayNightCycleValueChanged -= HandleDayNightCycleValueChanged;

            fpsController.OnJump -= HandleJump;
        }

        private void Start()
        {
            ambientEventInstance = RuntimeManager.CreateInstance(ambientEvent);
            ambientEventInstance.start();
        }

        private void OnDestroy()
        {
            ambientEventInstance.stop(STOP_MODE.ALLOWFADEOUT);      // pentru sunetele looped
            ambientEventInstance.release();
        }

        private void HandleFootstepDetected(AudioUtils.AudioSurfaceType type, float arg2)
        {
            EventInstance inst = RuntimeManager.CreateInstance(footstepEvent);  // ca Instantiate() din Unity dar pentru evenimente audio din FMOD

            String materialType = type.ToString();
            if (fpsController.isTouchingWater){
                Debug.Log("Is touching Water");
                materialType = "Water";
            }

            inst.setParameterByNameWithLabel("MaterialType", materialType);
            inst.start();   // eveniment de tip OneShot
            inst.release(); // eliberam memoria (distrugem instanta) dupa ce sunetul s-a terminat
        }

        private void HandleDayNightCycleValueChanged(float value)
        {
            ambientEventInstance.setParameterByName("AmbientBlend", value);
        }

        private void HandleJump(AudioUtils.AudioSurfaceType type)
        {
            EventInstance inst = RuntimeManager.CreateInstance(jumpEvent);
            inst.setParameterByNameWithLabel("MaterialType", type.ToString());
            inst.start();
            inst.release();
        }
    }
}

#endif