using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Chunk : MonoBehaviour
{
    private TerrainHandler terrain;
    public int xOrigin;
    public int yOrigin;
    public Biome biome;
    public void Create(TerrainHandler terrainHandler, int x, int y){

        terrain = terrainHandler;
        yOrigin = y;
        xOrigin = x;

        // Create all of the parts of the chunk
        transform.gameObject.AddComponent<TagHandler>();

        GameObject fg = new GameObject { name = "fg" };
        fg.AddComponent<Tilemap>();
        fg.AddComponent<TilemapRenderer>();
        fg.GetComponent<TilemapRenderer>().sortingLayerName = "Tiles-FG";
        fg.AddComponent<TilemapCollider2D>();
        fg.isStatic = true;
        fg.transform.parent = transform;

        GameObject bg = new GameObject { name = "bg" };
        bg.AddComponent<Tilemap>();
        bg.AddComponent<TilemapRenderer>();
        bg.GetComponent<TilemapRenderer>().sortingLayerName = "Tiles-BG";
        bg.isStatic = true;
        bg.GetComponent<Tilemap>().color = new Color(0.5f, 0.5f, 0.5f); // temporary, dims the background a bit
        bg.transform.parent = transform;

    }

    public bool InChunk(int x, int y){
        // Check if the coords are within this chunk
        if (x > xOrigin + terrain.world.chunkSize || x < xOrigin){
            return false;
        }
        if (y > yOrigin + terrain.world.chunkSize || y < yOrigin){
            return false;
        }
        return true;
    }

    public void PlaceTile(int x, int y, Tile tile, bool background=false){
        if (!InChunk(x,y)){throw new PositionNotInChunkException();}

        GameObject obj;

        if (background){obj = transform.Find("bg").gameObject;}
        else {obj = transform.Find("fg").gameObject;}

        obj.GetComponent<Tilemap>().SetTile(new Vector3Int(x, y, 0), tile);
    }

    public Tile GetTile(int x, int y, bool background=false){
        if (!InChunk(x,y)){throw new PositionNotInChunkException();}

        GameObject obj;

        if (background){obj = transform.Find("bg").gameObject;}
        else {obj = transform.Find("fg").gameObject;}

        return (Tile) obj.GetComponent<Tilemap>().GetTile(new Vector3Int(x, y, 0));
    }

    public void Generate( ){

        // What biome should this chunk be?
        Biome[] availableBiomes = new Biome[]{
            terrain.GetBiome(xOrigin+terrain.world.chunkSize,yOrigin+terrain.world.chunkSize,true),
            terrain.GetBiome(xOrigin+terrain.world.chunkSize,yOrigin-terrain.world.chunkSize,true),
            terrain.GetBiome(xOrigin-terrain.world.chunkSize,yOrigin+terrain.world.chunkSize,true),
            terrain.GetBiome(xOrigin-terrain.world.chunkSize,yOrigin-terrain.world.chunkSize,true),
            terrain.world.biomes[Random.Range(0, terrain.world.biomes.GetLength(0))]
        };

        int index = Random.Range(0,availableBiomes.Length);
        while (availableBiomes[index] == null){ // We can't have it be null, it might be null if the other chunks don't exist
            index = Random.Range(0,availableBiomes.Length);
        }
        biome = availableBiomes[index];

        // Ok we have a biome selected, now make some tiles!

        for (int x = xOrigin; x <= xOrigin + terrain.world.chunkSize; x++){
            for (int y = yOrigin; y <= yOrigin + terrain.world.chunkSize; y++){
                PlaceTile(x,y,biome.dirtTile);
            }
        }
    }
}