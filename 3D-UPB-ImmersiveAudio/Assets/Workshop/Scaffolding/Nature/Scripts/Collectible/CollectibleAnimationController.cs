using System;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace Workshop.Scaffolding.Nature.Scripts.Collectible
{
    public class CollectibleAnimationController : MonoBehaviour
    {
        [SerializeField, BoxGroup("Rotation")]            
        private float rotationSpeed       = 90f;
        
        [SerializeField, BoxGroup("Pulse scale")]         
        private float pulseScaleXZ        = 1.08f;
        
        [SerializeField, BoxGroup("Pulse scale")]         
        private float pulseScaleY         = 1.14f;
        
        [SerializeField, BoxGroup("Pulse scale")]         
        private float pulseDuration       = 0.9f;
        
        [SerializeField, BoxGroup("Pulse scale")]         
        private Ease pulseEaseIn          = Ease.InOutBack;
        
        [SerializeField, BoxGroup("Pulse scale")]         
        private Ease pulseEaseOut         = Ease.InOutBack;

        [SerializeField, BoxGroup("Collection animation")]
        private bool playCollectAnimation = true;
        
        [SerializeField, BoxGroup("Collection animation")] 
        private float collectScalePunch   = 1.4f;
        
        [SerializeField, BoxGroup("Collection animation")] 
        private float collectRiseDuration = 0.4f;
        
        [SerializeField, BoxGroup("Collection animation")] 
        private float collectFadeDelay    = 0.25f;
        
        [SerializeField, BoxGroup("Collection animation")]
        private float collectFadeDuration = 0.3f;

        private Vector3  baseScale;
        private Sequence idleSequence;

        private void Awake() => baseScale = transform.localScale;

        private void OnEnable() => PlayIdleAnimation();

        private void OnDisable()
        {
            idleSequence?.Kill();
            DOTween.Kill(transform);
        }

        private void PlayIdleAnimation()
        {
            idleSequence?.Kill();
            transform.localScale = baseScale;

            // Rotation.
            transform.DOLocalRotate(
                new Vector3(0f, 360f, 0f),
                360f / rotationSpeed,
                RotateMode.FastBeyond360
            ).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear).SetId(transform);

            // Squishy pulse scale.
            idleSequence = DOTween.Sequence();

            var pulsedScale = new Vector3(
                baseScale.x * pulseScaleXZ,
                baseScale.y * pulseScaleY,
                baseScale.z * pulseScaleXZ
            );

            // Continuous animation.
            idleSequence
                .Append(transform.DOScale(pulsedScale, pulseDuration).SetEase(pulseEaseIn))
                .Append(transform.DOScale(baseScale, pulseDuration).SetEase(pulseEaseOut))
                .SetLoops(-1, LoopType.Restart)
                .SetId(transform);
        }

        [Button] // For editor testing.
        public void PlayCollectAnimation(Action onComplete = null)
        {
            if (!playCollectAnimation)
            {
                onComplete?.Invoke();
                return;
            }

            // Kill idle.
            idleSequence?.Kill();
            DOTween.Kill(transform);

            var collectSequence = DOTween.Sequence();

            collectSequence
                // Quick punch scale up.
                .Append(transform.DOScale(baseScale * collectScalePunch, collectRiseDuration * 0.4f).SetEase(Ease.OutBack))
                // Rise upward slightly.
                .Join(transform.DOLocalMoveY(transform.localPosition.y + 0.4f, collectRiseDuration).SetEase(Ease.OutCubic))
                // Continue rotating faster.
                .Join(transform.DOLocalRotate(new Vector3(0f, 720f, 0f), collectRiseDuration, RotateMode.FastBeyond360).SetEase(Ease.InCubic))
                // Fade scale to zero after delay.
                .AppendInterval(collectFadeDelay)
                .Append(transform.DOScale(Vector3.zero, collectFadeDuration).SetEase(Ease.InBack))
                .OnComplete(() => onComplete?.Invoke());
        }
    }
}