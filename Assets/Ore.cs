using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class Ore
{
    public string name;
    public Tile ore;
    [Range(0, 1)]
    public float rarity;
    [Range(0, 1)]
    public float size;
    public int maxSpawnHeight;
    public Texture2D spreadTexture;
}
