using System;
using NaughtyAttributes;
using UnityEngine;
using Workshop.Scaffolding.Nature.Scripts.Player;
using static Workshop.Scaffolding.Nature.Scripts.Audio.AudioUtils;

namespace Workshop.Scaffolding.Nature.Scripts.Audio
{
    public class AudioOptionsUIController : MonoBehaviour
    {
        [SerializeField, BoxGroup("UI references")]
        private RectTransform optionsPanel;
        
        [SerializeField, BoxGroup("UI references")]
        private FPSController fpsController;

        [SerializeField, BoxGroup("UI references")]
        private AudioOptionsSlider[] soundOptionSliders;
        
        public event Action<AudioOptionType, float> OnAudioOptionChanged;
        
        private void Awake()
        {
            optionsPanel.gameObject.SetActive(false);
            fpsController.SetCursorLockState(true);
            foreach (var slider in soundOptionSliders)
            {
                slider.OnOptionChanged += HandleOptionChanged;
            }
        }

        private void OnDestroy()
        {
            foreach (var slider in soundOptionSliders)
            {
                slider.OnOptionChanged -= HandleOptionChanged;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                var isPanelActive = optionsPanel.gameObject.activeSelf;
                optionsPanel.gameObject.SetActive(!isPanelActive);
                fpsController.SetCursorLockState(isPanelActive);
            }
        }
        
        private void HandleOptionChanged(AudioOptionType type, float value) => OnAudioOptionChanged?.Invoke(type, value);
    }
}
