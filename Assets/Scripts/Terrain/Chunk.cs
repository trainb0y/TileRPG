 using System;
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
        if (x > xOrigin + terrain.world.chunkSize -1 || x < xOrigin){
            return false;
        }
        if (y > yOrigin + terrain.world.chunkSize -1 || y < yOrigin){
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


    public void Generate(){
        // Make the chunk actually have terrain

        // What biome should this chunk be?
        Biome[] availableBiomes = new Biome[]{
            terrain.GetBiome(xOrigin+terrain.world.chunkSize,yOrigin+terrain.world.chunkSize,true),
            terrain.GetBiome(xOrigin+terrain.world.chunkSize,yOrigin-terrain.world.chunkSize,true),
            terrain.GetBiome(xOrigin-terrain.world.chunkSize,yOrigin+terrain.world.chunkSize,true),
            terrain.GetBiome(xOrigin-terrain.world.chunkSize,yOrigin-terrain.world.chunkSize,true),

            terrain.GetBiome(xOrigin,yOrigin+terrain.world.chunkSize,true),
            terrain.GetBiome(xOrigin,yOrigin-terrain.world.chunkSize,true),
            terrain.GetBiome(xOrigin+terrain.world.chunkSize,yOrigin,true),
            terrain.GetBiome(xOrigin-terrain.world.chunkSize,yOrigin,true),
        };

        // For the first chunk we cant randomly select a neighbor one, as there are no neighbors and
        // finding a non-null one makes an infinite loop, so we need to pick a random biome

        // I don't like this, with the nonNullBiome stuff can it be more efficient?
        bool allNullBiomes = true;
        foreach (Biome b in availableBiomes){if (b != null){allNullBiomes = false;}}

        System.Random rand = new System.Random(terrain.seed.GetHashCode());

        if (allNullBiomes){biome = terrain.world.biomes[rand.Next(0, terrain.world.biomes.GetLength(0))];}
        
        else{
            int index = rand.Next(0,availableBiomes.Length);
            while (availableBiomes[index] == null){ // We can't have it be null, it might be null if the other chunks don't exist
                index = rand.Next(0,availableBiomes.Length);
            }
            biome = availableBiomes[index];

            // Maybe it should be different from the surroundings
            if (rand.Next(0, terrain.world.biomeRate) == terrain.world.biomeRate - 1){
                Debug.Log("Randomly choosing world biome");
                biome = terrain.world.biomes[rand.Next(0, terrain.world.biomes.GetLength(0))];
            } 
        }
       
        // Ok we have a biome selected, now make some tiles!

        for (int x = xOrigin; x < xOrigin + terrain.world.chunkSize; x++){
            // Figure out the surface height at this x
           float height = Mathf.PerlinNoise((x + (float) terrain.seed) * terrain.world.terrainFrequency, (float) terrain.seed * terrain.world.terrainFrequency) * 
           biome.heightMultiplier + biome.heightAddition + terrain.world.worldGenHeight;

            for (int y = yOrigin; y < yOrigin + terrain.world.chunkSize; y++){
                if (y < height){
                    if (y < height - biome.dirtHeight)
                    {
                        PlaceTile(x,y,biome.stoneTile); // foreground
                        PlaceTile(x,y,biome.stoneTile, true); // background
                    }
                    else{
                        PlaceTile(x,y,biome.dirtTile);
                        PlaceTile(x,y,biome.dirtTile,true);
                    }
                }
            }
        }

        // We have the basic shape, now we need ores

        // Maybe we can just iterate over the chunk's tiles once?
        // TODO: make this better lol
        foreach (Ore ore in biome.ores){
            for (int x = xOrigin; x < xOrigin + terrain.world.chunkSize; x++){
                for (int y = yOrigin; y < yOrigin + terrain.world.chunkSize; y++){ // Is there a better way to iterate over the tiles?
                    if (y < ore.maxSpawnHeight + terrain.world.worldGenHeight){
                        if (Mathf.PerlinNoise((x + terrain.seed) * ore.rarity, (y + terrain.seed) * ore.rarity) > ore.size){
                            PlaceTile(x,y, ore.tile); // foreground
                            PlaceTile(x,y, ore.tile, true); // background
                        }
                    }
                }
            }
        }


        // Next are caves
        // Because getting cellular automata to work with infinite generation is nigh impossible, 
        // we're just going to stack a few layers of perlin noise
        if (biome.generateCaves){
            for (int x = xOrigin; x < xOrigin + terrain.world.chunkSize; x++){
                for (int y = yOrigin; y < yOrigin + terrain.world.chunkSize; y++){ // Is there a better way to iterate over the tiles?
                    // To prevent the caves from making swiss cheese of the ground, lets make another perlin noise
                    // wave that determines the max height for the caves.
                    if (y < Mathf.PerlinNoise((x + (float) terrain.seed) * terrain.world.caveMaxHeightFrequency, (float) terrain.seed * terrain.world.caveMaxHeightFrequency) * terrain.world.caveMaxHeightAmplitude + terrain.world.caveBaseMaxHeight){
                        int i = 1;
                        foreach (CaveLayer layer in terrain.world.caveLayers){
                            if(Mathf.PerlinNoise((x+(terrain.seed*i))*layer.frequency,(y+(terrain.seed*i))*layer.frequency) < layer.cutoff){
                                PlaceTile(x,y,null);
                            }
                            i++; // Might just be better do do a for i... not foreach, but eh
                        }
                    }
                }   
            }
        }
    }
}