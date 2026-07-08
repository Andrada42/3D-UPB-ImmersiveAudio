using UnityEngine;

namespace Workshop.Scaffolding.Nature.Scripts.Collectible
{
    public class CollectibleRemover : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                CollectibleTracker.Instance.RemoveCollectible();
            }
        }
    }
}
