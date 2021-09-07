using UnityEngine.Tilemaps;
using UnityEngine;

[CreateAssetMenu()]
public class Biome : ScriptableObject
{
    [Header("Generation")]
    public bool generateCaves = true;
    public int heightAddition;
    public int heightMultiplier;

    public Tile grassTile;
    public Tile dirtTile;
    public Tile stoneTile;

    public int dirtHeight = 5;

    public Ore[] ores;

    public Background[] backgrounds;

}
