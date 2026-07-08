using UnityEngine;
using static Workshop.Scaffolding.Nature.Scripts.Audio.AudioUtils;

namespace Workshop.Scaffolding.Nature.Scripts.Player
{
    public class TerrainFootstepSurfaceProvider : MonoBehaviour, IFootstepSurfaceProvider
    {
        public AudioSurfaceType GetSurface(Collider groundCollider, Vector3 hitPoint)
        {
            // First check if we're on a tagged object.
            if (groundCollider.CompareTag("Wood"))
            {
                return AudioSurfaceType.Wood;
            }
            // Check if we hit terrain.
            var terrain = groundCollider.GetComponent<Terrain>();
            if (terrain != null)
            {
                // Convert hit point to terrain coordinates.
                Vector3 terrainPos = hitPoint - terrain.transform.position;
                Vector2 normalizedPos = new Vector2(
                    terrainPos.x / terrain.terrainData.size.x,
                    terrainPos.z / terrain.terrainData.size.z);

                // Sample the dominant texture.
                var splatmapData = terrain.terrainData.GetAlphamaps(
                    Mathf.FloorToInt(normalizedPos.x * terrain.terrainData.alphamapWidth),
                    Mathf.FloorToInt(normalizedPos.y * terrain.terrainData.alphamapHeight),
                    1, 1);

                // Find dominant texture index.
                int dominantTexture = 0;
                float maxMix = 0;

                for (var i = 0; i < splatmapData.GetLength(2); i++)
                {
                    if (splatmapData[0, 0, i] > maxMix)
                    {
                        maxMix = splatmapData[0, 0, i];
                        dominantTexture = i;
                    }
                }

                // Map texture index to surface type (depends on Terrain painting texture order).
                return dominantTexture switch
                {
                    0 => AudioSurfaceType.Dirt,  // Texture is "Grass", using "Dirt" as placeholder.
                    1 => AudioSurfaceType.Dirt,
                    2 => AudioSurfaceType.Stone,
                    _ => AudioSurfaceType.None
                };
            }

            return AudioSurfaceType.None;
        }
    }
}
