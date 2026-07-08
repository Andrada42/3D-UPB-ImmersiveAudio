#if STEAMAUDIO_ENABLED

using NaughtyAttributes;
using SteamAudio;
using UnityEngine;

namespace Workshop.Scaffolding.SteamAudio.Scripts.Utils
{
    [RequireComponent(typeof(SteamAudioProbeBatch))]
    public class SteamAudioPathingBoundsHelper : MonoBehaviour
    {
        [Button]
        private void SetupPathingBounds()
        {
            // Get all objects that have a `SteamAudioGeometry` component attached.
            var geometrySet = FindObjectsByType<SteamAudioGeometry>(FindObjectsSortMode.None);
            
            // Build accumulated bounds from all meshes from the mesh setup.
            var bounds = new Bounds();
            foreach (var geometry in geometrySet)
            {
                var meshRenderer = geometry.GetComponent<MeshRenderer>();
                if (meshRenderer == null) continue;
                bounds.Encapsulate(meshRenderer.bounds);
            }
            // Set the bounds.
            transform.position   = bounds.center;
            transform.localScale = bounds.size;
        }
    }
}

#endif