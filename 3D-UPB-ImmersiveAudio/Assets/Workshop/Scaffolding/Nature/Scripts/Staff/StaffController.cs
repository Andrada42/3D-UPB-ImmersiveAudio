using DG.Tweening;
using FMODUnity;
using NaughtyAttributes;
using System;
using UnityEngine;
using Workshop.Scaffolding.Nature.Scripts.Audio;

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

        [SerializeField, BoxGroup("FMOD Events")]
        private EventReference timelineBeatEvent;

        private Vector3 _initialScale;
        private Tween   _lightTween;
        private Tween   _scaleTween;

        private void Awake()
        {
            _initialScale = staffCrystal.localScale;
            staffLight.intensity = 0f;

            staffLight.color = Color.white;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            TimelineBeatService.OnBeat += HandleBeat;
            TimelineBeatService.Start(timelineBeatEvent, transform.position);

            TimelineBeatService.OnMarker += HandleMarker;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            TimelineBeatService.OnBeat -= HandleBeat;
            TimelineBeatService.Stop();

            TimelineBeatService.OnMarker -= HandleMarker;

            // Don't leave a pulse mid-flight once the player walks away.
            _lightTween?.Kill();
            _scaleTween?.Kill();
            staffLight.intensity    = 0f;
            staffCrystal.localScale = _initialScale;
            staffLight.color        = Color.white;
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

        private void HandleMarker(string markerName)
        {
            Color staffColor;
            switch (markerName)
            {
                case "Violet":
                    staffColor = Color.violet;
                    break;
                case "Red":
                    staffColor = Color.red;
                    break;
                case "Orange":
                    staffColor = Color.orange;
                    break;
                case "Turquoise":
                    staffColor = Color.turquoise;
                    break;
                case "Green":
                    staffColor = Color.green;
                    break;
                default:
                    staffColor = Color.white;
                    break;
            }
            staffLight.color = staffColor;
        }
    }
}
