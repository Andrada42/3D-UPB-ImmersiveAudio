using System;
using UnityEngine;
using Workshop.Scaffolding.Nature.Scripts.Utils;

namespace Workshop.Scaffolding.Nature.Scripts.Collectible
{
    public struct CollectibleData
    {
        public int       Count;
        public AudioClip Clip;
        public Vector3   Position;
    }

    public class CollectibleTracker : Singleton<CollectibleTracker>
    {
        private int collectibleCount;
        
        public event Action<CollectibleData> OnCollectibleGathered;
        public event Action<int>             OnCollectibleRemoved;
        
        public void RegisterCollection(CollectibleData data)
        {
            collectibleCount++;
            data.Count = collectibleCount;
            OnCollectibleGathered?.Invoke(data);
            
            Debug.Log($"[Workshop] [CollectibleTracker] Collectible count: {collectibleCount}");
        }

        public void RemoveCollectible()
        {
            if (collectibleCount <= 0) return;
            
            collectibleCount--;
            OnCollectibleRemoved?.Invoke(collectibleCount);

            Debug.Log($"[Workshop] [CollectibleTracker] Collectible count: {collectibleCount}");
        }
    }
}
