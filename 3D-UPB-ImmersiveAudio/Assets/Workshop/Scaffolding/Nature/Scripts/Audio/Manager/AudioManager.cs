using NaughtyAttributes;
using UnityEngine;
using Workshop.Scaffolding.Nature.Scripts.Gameplay;
using Workshop.Scaffolding.Nature.Scripts.Player;

namespace Workshop.Scaffolding.Nature.Scripts.Audio.Manager
{
    public abstract class AudioManager : MonoBehaviour
    {
        [SerializeField, BoxGroup("External components")]
        protected FPSController fpsController;

        [SerializeField, BoxGroup("External components")]
        protected DayNightCycleController dayNightCycleController;

        [SerializeField, BoxGroup("External components")]
        protected WaterVolumeDetector waterVolumeDetector;

        [SerializeField, BoxGroup("External components")]
        protected AudioOptionsUIController audioOptionsUIController;

        // Runs when the inspector is refreshed.
        protected virtual void OnValidate()
        {
            if (Application.isPlaying) return;
            
            // `??=` is a null-coalescing assignment operator. Reads as: "If the object is null, then assign it via right-hand operation".
            fpsController            ??= FindAnyObjectByType<FPSController>();
            dayNightCycleController  ??= FindAnyObjectByType<DayNightCycleController>();
            waterVolumeDetector      ??= FindAnyObjectByType<WaterVolumeDetector>();
            audioOptionsUIController ??= FindAnyObjectByType<AudioOptionsUIController>();
        }
    }
}
