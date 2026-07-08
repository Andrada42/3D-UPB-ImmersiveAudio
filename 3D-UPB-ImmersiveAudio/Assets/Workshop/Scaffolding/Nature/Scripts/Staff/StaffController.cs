using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace Workshop.Scaffolding.Nature.Scripts.Staff
{
    public class StaffController : MonoBehaviour
    {
        [SerializeField]
        [BoxGroup("Staff components")]
        private Light staffLight;

        [SerializeField]
        [BoxGroup("Staff components")]
        private Transform staffCrystal;

        [SerializeField]
        [BoxGroup("Settings")]
        private float lightDecayDuration = 0.3f;

        [SerializeField]
        [BoxGroup("Settings")]
        private float maxScaleAnimation = 1f;

        [SerializeField]
        [BoxGroup("Settings")]
        private float scalePunchDuration = 0.4f;

        private Vector3 _initialScale;
        private Tween   _lightTween;
        private Tween   _scaleTween;

        private void Awake()
        {
            _initialScale = staffCrystal.localScale;
            staffLight.intensity = 0f;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            // Don't leave a pulse mid-flight once the player walks away.
            _lightTween?.Kill();
            _scaleTween?.Kill();
            staffLight.intensity    = 0f;
            staffCrystal.localScale = _initialScale;
        }

        private void HandleBeat(int bar, int beat)
        {
            if (bar == 0 && beat == 0) return;
            var maxIntensity = beat == 1 ? 15f : 5f;
            
            _lightTween?.Kill();
            staffLight.intensity = maxIntensity;
            _lightTween = staffLight.DOIntensity(0f, lightDecayDuration).SetEase(Ease.OutQuad);

            _scaleTween?.Kill();
            staffCrystal.localScale = _initialScale;
            _scaleTween = staffCrystal.DOPunchScale(Vector3.one * maxScaleAnimation, scalePunchDuration);
        }
    }
}
