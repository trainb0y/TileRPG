using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TerrainHandler : MonoBehaviour
{
    public World world;
    private Dictionary<int[], GameObject> chunks; // no such thing as a 2d list, and don't want to use array so /shrug
    private float seed; // Copied from world seed or generated

     //float v = Mathf.PerlinNoise((x + seed) * frequency, (y + seed) * frequency);
    
    void Start()
    {
        chunks = new Dictionary<int[], GameObject>();
        if (world.seed == null) {
            seed = Random.Range(-1000000, 1000000);
        }
        else{
            seed = (float) world.seed;
        }

        GenerateChunk(0,0);
    }


    public GameObject GetChunkObject(int x, int y, bool allowNull=false)
    {
        // Return the chunk at the given coordinates
        try
        {
            float chunkX = (Mathf.Round(x / world.chunkSize) * world.chunkSize);
            float chunkY = (Mathf.Round(y / world.chunkSize) * world.chunkSize);
            chunkY /= world.chunkSize;
            chunkX /= world.chunkSize;

            return chunks[new int[]{
                (int)chunkX,
                (int)chunkY
            }];
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
            return;
        }

        // First, create the chunk
        GameObject chunk = new GameObject();
        chunk.name = "chunk_" + x.ToString() + "_" + y.ToString();
        chunk.transform.parent = transform;
        chunk.isStatic = true;

        chunks.Add(new int[]{
            (int)x,
            (int)y
        }, chunk);

        Chunk script = chunk.AddComponent<Chunk>();
        script.Create(this, x ,y);
        script.Generate();

    }

}
