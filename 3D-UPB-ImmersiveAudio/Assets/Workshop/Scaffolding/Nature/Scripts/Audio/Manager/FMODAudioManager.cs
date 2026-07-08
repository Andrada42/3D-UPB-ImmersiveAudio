#if FMOD_INSTALLED

using FMODUnity;
using NaughtyAttributes;
using UnityEngine;

namespace Workshop.Scaffolding.Nature.Scripts.Audio.Manager
{
    public class FMODAudioManager : AudioManager
    {
        [SerializeField, BoxGroup("FMOD Events")]
        private EventReference footstepEvent;

        [SerializeField, BoxGroup("FMOD Events")]
        private EventReference ambientEvent;
        
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
    }
}

#endif