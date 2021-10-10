using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Collections.Generic;

public class TerrainHandler : MonoBehaviour
{
    public World world;
    private Dictionary<Tuple<int,int>, GameObject> chunks; // no such thing as a 2d list, and don't want to use array so /shrug
    private float seed; // Copied from world seed or generated

     //float v = Mathf.PerlinNoise((x + seed) * frequency, (y + seed) * frequency);
    
    void Start()
    {
        chunks = new Dictionary<Tuple<int,int>, GameObject>();
        if (world.seed == null) {
            seed = UnityEngine.Random.Range(-1000000, 1000000);
        }
        else{
            seed = (float) world.seed;
        }

        for (int x = 0; x < 200; x+=world.chunkSize){
            for (int y = 0; y < 200; y+=world.chunkSize){
                GenerateChunk(x,y);
            }
        }
    }


    public GameObject GetChunkObject(int x, int y, bool allowNull=false)
    {
        // Return the chunk at the given coordinates
        try
        {
            x -=  x % world.chunkSize;
            y -=  y % world.chunkSize;

            return chunks[new Tuple<int, int>(x,y)];
        }
        catch (System.Collections.Generic.KeyNotFoundException){
            if (!allowNull){
                throw new ChunkNotFoundException();
            }
            else{
                return null;
            }
        }
    }

    public Chunk GetChunk(int x, int y, bool allowNull=false){
        GameObject chunkObj = GetChunkObject(x,y,allowNull);
        if (chunkObj == null){ // Have to avoid NullRefrenceException when doing GetComponent<>()
            return null;
        }
        return chunkObj.GetComponent<Chunk>();
        
    }

    public Biome GetBiome(GameObject chunkObject){
        return chunkObject.GetComponent<Chunk>().biome;
    }

    public Biome GetBiome(int x, int y, bool allowNullChunk=false){
        Chunk chunk = GetChunk(x,y,allowNullChunk);
        if (chunk == null){ // Have to avoid NullRefrenceException when accessing biome
            return null;
        }
        return chunk.biome;
    }


    public void PlaceWorldTile(int x, int y, Tile tile, bool background=false)
    {
        GetChunk(x,y).PlaceTile(x,y,tile,background);
    }
    
    public Tile GetWorldTile(int x, int y, bool background = false)
    {
        // Get the tile at the given x and y coordinates
        return GetChunk(x,y).GetTile(x,y,background);
    }



    public void GenerateChunk(int x, int y){
        // Assume x,y are the minimum values

        if (GetChunkObject(x,y,true) != null){
            throw new ChunkExistsException();
        }

        // First, create the chunk
        GameObject chunk = new GameObject();
        chunk.name = "chunk_" + x.ToString() + "_" + y.ToString();
        chunk.transform.parent = transform;
        chunk.isStatic = true;

        chunks.Add(new Tuple<int, int>(x,y), chunk);

        Chunk script = chunk.AddComponent<Chunk>();
        script.Create(this, x ,y);
        script.Generate();

    }

}
