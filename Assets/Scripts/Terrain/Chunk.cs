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

    public Dictionary<Tuple<int,int>, bool> MooreSmoothing(Dictionary<Tuple<int,int>, bool> map, int x, int y){
        // Returns true if it should be a tile, else false


        // https://blog.unity.com/technology/procedural-patterns-to-use-with-tilemaps-part-ii
        /*
        If a neighbour is a tile, add one to the count
        If a neighbour is air, don't
        If the cell has more than 4 surrounding tiles, make the cell a tile.
        If the cell has exactly 4 surrounding tiles, leave the tile alone.
        */
        Dictionary<Tuple<int, int>, bool> newMap = new Dictionary<Tuple<int, int>, bool>();

        for (int x1 = x - (terrain.world.chunkSize * 2); x1 < x + (terrain.world.chunkSize * 2); x1++)
        {
            for (int y1 = y - (terrain.world.chunkSize * 2); y1 < y + (terrain.world.chunkSize * 2); y1++)
            {
                // For each tile
                 int count = 0;
                for (int x2 = x1 - 1; x2 <= x1 + 1; x2++)
                {
                    for (int y2 = y1 - 1; y2 <= y1 + 1; y2++)
                    {      
                        // For each tile surrounding that tile
                        // x1, y1 are badly named, ik, but they are the coordinates of the neighbor we are checking
                        // Don't include this tile
                        if(x2 != x1 || y2 != y1)
                        { 
                            try{
                                if (map[new Tuple<int, int>(x2,y2)]){count += 1;}  // This shouldn't happen, but it does
                            }
                            catch (System.Collections.Generic.KeyNotFoundException){
                                map[new Tuple<int, int>(x2,y2)] = false;
                            } // TODO: Investigate why this would ever happen
                        }
                    }
                }
                newMap[new Tuple<int, int>(x1,y1)] = count > 4;
            }
        }
        return newMap;
    }
 
    public Dictionary<Tuple<int,int>, bool> GenerateCaves(int x, int y){
        // Generate caves 
        Dictionary<Tuple<int,int>, bool> map = new Dictionary<Tuple<int,int>, bool>();

        for (int x1 = x - (terrain.world.chunkSize * 3); x1 < x + (terrain.world.chunkSize * 3); x1++)
        {
            for (int y1 = y - (terrain.world.chunkSize * 3); y1 < y + (terrain.world.chunkSize * 3); y1++) // the multiplier here needs to be bigger than the one in smoothing
            {
                //Randomly generate the grid
                //System.Random rand = new System.Random(terrain.seed.GetHashCode());
                //map[new Tuple<int, int>(x1,y1)] = (rand.Next(0, 100) < terrain.world.caveFillPercent);
                // RAND.NEXT IS NOT GOING TO WORK AS IT DOESN'T COMPENSATE FOR THE COORDINATES OF THIS CHUNK
                // Going to try perlin noise, though it might do weird things

                // Can't leave the perlin noise on its own, or else it generates bad perlin noise caves, so we have to uh, do something
                map[new Tuple<int, int>(x1,y1)] = (Mathf.PerlinNoise((x1*3)+terrain.seed,(y1*3)+terrain.seed) * 100f < terrain.world.caveFillPercent);
                //map[new Tuple<int, int>(x1,y1)] = UnityEngine.Random.Range(0,100) > 35;
            }
        }

        
        for (int i = 0; i <= terrain.world.caveSmoothAmount; i++){
            map = MooreSmoothing(map,x,y); // Smooth the map
        }
    
        return map;

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

        for (int x = xOrigin; x <= xOrigin + terrain.world.chunkSize - 1; x++){
            // Figure out the surface height at this x
           float height = Mathf.PerlinNoise((x + (float) terrain.seed) * terrain.world.terrainFreq, (float) terrain.seed * terrain.world.terrainFreq) * 
           biome.heightMultiplier + biome.heightAddition + terrain.world.worldGenHeight;

            for (int y = yOrigin; y <= yOrigin + terrain.world.chunkSize - 1; y++){
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
            for (int x = xOrigin; x <= xOrigin + terrain.world.chunkSize - 1; x++){
                for (int y = yOrigin; y <= yOrigin + terrain.world.chunkSize - 1; y++){ // Is there a better way to iterate over the tiles?
                    if (y < ore.maxSpawnHeight + terrain.world.worldGenHeight){
                        if (Mathf.PerlinNoise((x + terrain.seed) * ore.rarity, (y + terrain.seed) * ore.rarity) > ore.size){
                            PlaceTile(x,y, ore.tile); // foreground
                            PlaceTile(x,y, ore.tile, true); // background
                        }
                    }
                }
            }
        }


        // Next is caves. Start with some basic noise
        // Basically following https://blog.unity.com/technology/procedural-patterns-to-use-with-tilemaps-part-ii

        /*
        So, we can't rely on any of the surrounding chunks existing, but we still need to pretend they do.
        DISCLAIMER: anyone who gives a crap about optimization please look away and save yourself.

        In order to have a decent idea of what the surrounding chunks would look like if they were generated we can generate
        noise for a couple layers of fake chunk surrounding the one we actually care about. The smoothing will mess up the edges
        of those fake chunks, but since they aren't in the game, we don't care. As the noise is seeded, and the smoothing has the same
        result every time, the caves in different chunks *should* in theory line up.
        */

        Dictionary<Tuple<int,int>, bool> caves = GenerateCaves(xOrigin,yOrigin); 
        for (int x = xOrigin; x <= xOrigin + terrain.world.chunkSize - 1; x++){
            for (int y = yOrigin; y <= yOrigin + terrain.world.chunkSize - 1; y++){ // Is there a better way to iterate over the tiles?
                if (!caves[new Tuple<int,int>(x,y)]){
                    PlaceTile(x,y, null);
                }
            }
        }
    }
}