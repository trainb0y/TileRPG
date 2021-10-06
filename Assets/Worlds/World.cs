using UnityEngine;

[CreateAssetMenu()]
public class World : ScriptableObject
{
    [Header("Generation")]
    public int worldGenHeight = 500;
    public int chunkSize = 10;
    public float caveFreq = 0.05f;
    public float terrainFreq = 0.05f;
    public float caveCutoff = 0.3f;
    public float? seed;


    [Header("Biomes")]
    public Biome[] biomes;


}
