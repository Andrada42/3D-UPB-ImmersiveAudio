using NaughtyAttributes;
using UnityEngine;

namespace Workshop.Scaffolding.Nature.Scripts.Collectible
{
    [RequireComponent(typeof(SphereCollider))]
    public class CollectibleController : MonoBehaviour
    {
        [SerializeField, BoxGroup("External references")]
        private CollectibleAnimationController animationController;
        
        [SerializeField, BoxGroup("External references")]
        private AudioClip audioClip;

        private bool _alreadyCollected;
        
        private void OnTriggerEnter(Collider other)
        {
            if (_alreadyCollected)           return;
            if (!other.CompareTag("Player")) return;
            
            Collect();
        }
        
        private void Collect()
        {
            animationController.PlayCollectAnimation(() => Destroy(gameObject));
            
            CollectibleTracker.Instance.RegisterCollection(new CollectibleData
            {
                Clip     = audioClip,
                Position = transform.position
            });
            
            _alreadyCollected = true;
        }
    }
}
