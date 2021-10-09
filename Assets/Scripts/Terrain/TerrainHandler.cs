using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TerrainHandler : MonoBehaviour
{
    public World world;
    public Tile placeholderTile;
    private Dictionary<int[], GameObject> chunks; // no such thing as a 2d list, and don't want to use array so /shrug
    private Grid grid;

    private float seed; // Copied from world seed or generated


    /*
     
    The world is divided into chunks
    Each chunk has 2 tilemaps as children, one for foreground

    The chunks are gameobjects

    Each chunk has 2 tilemaps as children, one for foreground
    one for background.

    This comment is pointless

     */

     //float v = Mathf.PerlinNoise((x + seed) * frequency, (y + seed) * frequency);
    


    void Start()
    {
        chunks = new Dictionary<int[], GameObject>();
        if (world.seed == null) {
            seed = Random.Range(-100000, 100000);
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

    public Chunk GetChunk(int x, int y){
        return GetChunkObject(x,y).GetComponent<Chunk>();
    }

    public Biome GetBiome(GameObject chunkObject){
        return chunkObject.GetComponent<Chunk>().biome;
    }

    public Biome GetBiome(int x, int y){
        return GetChunk(x,y).biome;
    }


    public void PlaceWorldTile(int x, int y, Tile tile, bool background=false)
    {
        // Place a tile at the given x and y. Automatically finds
        // the proper chunk.
        GameObject chunk = GetChunkObject(x,y);
        GameObject obj;
        if (background)
        {
            obj = chunk.transform.Find("bg").gameObject;
        }
        else
        {
            obj = chunk.transform.Find("fg").gameObject;
        }

        Tilemap tilemap = obj.GetComponent<Tilemap>();
        tilemap.SetTile(new Vector3Int(x, y, 0), tile);

    }
    
    Tile GetWorldTile(int x, int y, bool background = false)
    {
        // Get the tile at the given x and y coordinates
        GameObject chunk = GetChunkObject(x, y);
        if (chunk == null)
        {
            return null;
        }
        GameObject obj;
        if (background)
        {
            obj = chunk.transform.Find("bg").gameObject;
        }
        else
        {
            obj = chunk.transform.Find("fg").gameObject;
        }

        Tilemap tilemap = obj.GetComponent<Tilemap>();
        return (Tile) tilemap.GetTile(new Vector3Int(x, y, 0));
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
