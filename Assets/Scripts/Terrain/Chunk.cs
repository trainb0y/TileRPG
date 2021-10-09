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

    public void Generate( ){

        // What biome should this chunk be?
        Biome[] availableBiomes = new Biome[]{
            terrain.GetBiome(xOrigin+terrain.world.chunkSize,yOrigin+terrain.world.chunkSize),
            terrain.GetBiome(xOrigin+terrain.world.chunkSize,yOrigin-terrain.world.chunkSize),
            terrain.GetBiome(xOrigin-terrain.world.chunkSize,yOrigin+terrain.world.chunkSize),
            terrain.GetBiome(xOrigin-terrain.world.chunkSize,yOrigin-terrain.world.chunkSize),
            terrain.world.biomes[Random.Range(0, terrain.world.biomes.GetLength(0))]
        };

        int index = Random.Range(0,availableBiomes.Length);
        while (availableBiomes[index] == null){ // We can't have it be null, it might be null if the other chunks don't exist
            index = Random.Range(0,availableBiomes.Length);
        }
        biome = availableBiomes[index];

        // Ok we have a biome selected, now make some tiles!
    }

    public void PlaceTile(){

    }
}