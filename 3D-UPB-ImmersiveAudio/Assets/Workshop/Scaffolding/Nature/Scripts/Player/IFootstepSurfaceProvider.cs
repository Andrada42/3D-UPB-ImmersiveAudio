using UnityEngine;
using static Workshop.Scaffolding.Nature.Scripts.Audio.AudioUtils;

namespace Workshop.Scaffolding.Nature.Scripts.Player
{
    public interface IFootstepSurfaceProvider
    {
        AudioSurfaceType GetSurface(Collider groundCollider, Vector3 hitPoint);
    }
}
