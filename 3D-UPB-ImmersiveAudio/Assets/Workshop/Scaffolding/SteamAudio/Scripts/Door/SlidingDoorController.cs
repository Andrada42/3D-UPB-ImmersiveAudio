using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace Workshop.Scaffolding.SteamAudio.Scripts.Door
{
    public class SlidingDoorController : MonoBehaviour
    {
        private enum DoorState { Opened, Closed, Opening, Closing }
        
        [SerializeField]
        [BoxGroup("Door components")]
        private Transform doorFrameTransform;
        
        [SerializeField]
        [BoxGroup("Door components")]
        private Transform doorOpenedTransform;
        
        [SerializeField]
        [BoxGroup("Door components")]
        private Transform doorClosedTransform;
        
        [SerializeField]
        [BoxGroup("Settings")]
        private float doorSlidingTime = 1f;
        
        [SerializeField]
        [BoxGroup("Settings")]
        private Ease doorOpenEase = Ease.InOutCubic;
        
        [SerializeField]
        [BoxGroup("Settings")]
        private Ease doorCloseEase = Ease.InOutCubic;
        
        [SerializeField]
        [BoxGroup("Settings")]
        [ReadOnly]
        private DoorState doorState;
        
        private void Awake() => doorState = DoorState.Closed;

        private Tween _doorTween;
        
        public void ToggleDoor()
        {
            if (doorState is DoorState.Closed or DoorState.Closing)
            {
                OpenDoor();
                return;
            } 
            if (doorState is DoorState.Opened or DoorState.Opening)
            {
                CloseDoor();
            }
        }
        
        private void OpenDoor()
        {
            Debug.Log($"Open: Door state: {doorState}");
            if (doorState == DoorState.Opening) return;
            
            _doorTween?.Kill();
            _doorTween = doorFrameTransform
                .DOMove(doorOpenedTransform.position, doorSlidingTime)
                .SetEase(doorOpenEase)
                .OnComplete(() => doorState = DoorState.Opened);
            doorState = DoorState.Opening;
        }
        
        private void CloseDoor()
        {
            Debug.Log($"Close: Door state: {doorState}");
            if (doorState == DoorState.Closing) return;
            
            _doorTween?.Kill();
            _doorTween = doorFrameTransform
                .DOMove(doorClosedTransform.position, doorSlidingTime)
                .SetEase(doorCloseEase)
                .OnComplete(() => doorState = DoorState.Closed);
            doorState = DoorState.Closing;
        }
    }
}
