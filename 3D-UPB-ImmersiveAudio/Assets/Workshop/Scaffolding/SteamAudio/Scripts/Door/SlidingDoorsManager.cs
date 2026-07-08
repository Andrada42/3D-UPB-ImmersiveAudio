using NaughtyAttributes;
using UnityEngine;

namespace Workshop.Scaffolding.SteamAudio.Scripts.Door
{
    public class SlidingDoorsManager : MonoBehaviour
    {
        [SerializeField]
        [BoxGroup("External references")]
        private SlidingDoorController slidingDoor1;
        
        [SerializeField]
        [BoxGroup("External references")]
        private SlidingDoorController slidingDoor2;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) slidingDoor1.ToggleDoor();
            if (Input.GetKeyDown(KeyCode.Alpha2)) slidingDoor2.ToggleDoor();
        }
    }
}
