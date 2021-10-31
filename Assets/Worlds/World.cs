using UnityEngine;

[CreateAssetMenu()]
public class World : ScriptableObject
{
    [Header("World Settings")]
    public int worldGenHeight = 500;
    public int chunkSize = 10;
    public float terrainFreq = 0.05f;
    public float seed = -1;
    public int biomeRate = 10;
    public Biome[] biomes;

    [Header("Caves")]
    public int caveNoiseLayers = 3;
    public float caveCutoff = 0.3f;
    public float caveFrequency = 0.4f;
}
