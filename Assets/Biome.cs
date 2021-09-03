using UnityEngine.Tilemaps;
using UnityEngine;

[System.Serializable]
public class Biome 
{
    [Header("General Settings")]
    public string name;

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
