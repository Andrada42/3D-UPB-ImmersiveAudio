using System;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Workshop.Scaffolding.Nature.Scripts.Audio.AudioUtils;

namespace Workshop.Scaffolding.Nature.Scripts.Audio
{
    public class AudioOptionsSlider : MonoBehaviour
    {
        [SerializeField, BoxGroup("Settings")]
        private AudioOptionType type;
        
        [SerializeField, BoxGroup("Settings"), Range(0f, 1f)]
        private float defaultValue;
        
        [SerializeField, BoxGroup("UI references")] 
        private Slider slider;
        
        [SerializeField, BoxGroup("UI references")]
        private TextMeshProUGUI label;
        
        [SerializeField, BoxGroup("UI references")]
        private TextMeshProUGUI value;
        
        public event Action<AudioOptionType, float> OnOptionChanged;

        private void Awake()
        {
            label.text   = type.ToString();
            slider.value = defaultValue;
            value.text   = defaultValue.ToString("0.00");
            
            slider.onValueChanged.AddListener(OnSliderValueChanged);
        }

        private void OnDestroy() => slider.onValueChanged.RemoveListener(OnSliderValueChanged);

        private void OnSliderValueChanged(float sliderValue)
        {
            value.text = sliderValue.ToString("0.00");
            OnOptionChanged?.Invoke(type, sliderValue);
        }
    }
}
