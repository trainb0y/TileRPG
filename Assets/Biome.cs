using UnityEngine.Tilemaps;

[System.Serializable]
public class Biome 
{ 
    public string name;
    public bool generateCaves = true;
    public int heightAddition;
    public int heightMultiplier;

    public Tile grassTile;
    public Tile dirtTile;
    public Tile stoneTile;

    public int dirtHeight = 5;

    public Ore[] ores;
}
