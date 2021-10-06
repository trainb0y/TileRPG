using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainHandler : MonoBehaviour
{
    public World world;
    public Tile placeholderTile;
    private GameObject[,] chunks;
    private Grid grid;

    private float seed; // Copied from world seed or generated


    /*
     
    The world is divided into chunks

    The chunks are gameobjects

    Each chunk has 2 tilemaps as children, one for foreground
    one for background.

    This comment is pointless

     */
    


    void Start()
    {
        if (world.seed == null) {
            seed = Random.Range(-100000, 100000);
        }
        else{
            seed = (float) world.seed;
        }


        /*

        foreach (Biome biome in world.biomes)
        {
            foreach (Ore ore in biome.ores)
            {
                ore.spreadTexture = new Texture2D(xMax, yMax);
                GenerateNoiseTexture(ore.rarity, ore.size, ore.spreadTexture);
                // Generate ore spawn textures for each ore
            }
        }

        // Messing with this order will probably cause a lot of issues.
        */
    }


    public void GenerateNoiseTexture(float frequency, float limit, Texture2D noiseTexture)
    {
        // Make a perlin noise texture
        for (int x = 0; x < noiseTexture.width; x++)
        {
            for (int y = 0; y < noiseTexture.height; y++)
            {
                float v = Mathf.PerlinNoise((x + seed) * frequency, (y + seed) * frequency);
                if (v > limit)
                {
                    noiseTexture.SetPixel(x, y, Color.white);
                }
                else
                {
                    noiseTexture.SetPixel(x, y, Color.black);
                }

            }
        }
        noiseTexture.Apply();
    }
    public GameObject GetChunk(int x, int y)
    {
        // Return the chunk at the given coordinates
        try
        {
            float chunkX = (Mathf.Round(x / world.chunkSize) * world.chunkSize);
            float chunkY = (Mathf.Round(y / world.chunkSize) * world.chunkSize);
            chunkY /= world.chunkSize;
            chunkX /= world.chunkSize;

            return chunks[(int)chunkX, (int)chunkY];
        }
        catch (System.IndexOutOfRangeException){
            return null;
        }
        
    }

    public Biome GetBiome(GameObject chunk)
    {
        // Return the biome of the chunk

        foreach (Biome biome in world.biomes)
        {
            if (chunk.GetComponent<TagHandler>().HasTag(biome.name))
            {
                return biome;
            }
        }
        Debug.LogWarning("No biome for " + chunk.name);
        return null;
    }
    public void PlaceTile(int x, int y, Tile tile, bool background=false)
    {
        // Place a tile at the given x and y. Automatically finds
        // the proper chunk.
        GameObject chunk = GetChunk(x,y);
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
    
    Tile GetTile(int x, int y, bool background = false)
    {
        // Get the tile at the given x and y coordinates
        GameObject chunk = GetChunk(x, y);
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

    /*
    void GenerateTerrain()
    {
        // Place placeholder tiles in the shape of the terrain

        //print("Generating terrain");
        for (int x = 0; x < xMax; x++)
        {
            //print("x: " + x.ToString());
            Biome biome = GetBiome(GetChunk(x, 0));
            //print("Biome: " + biome.name);

            int heightMultiplier = biome.heightMultiplier;
            int heightAddition = biome.heightAddition;

            float height = Mathf.PerlinNoise((x + seed) * terrainFreq, seed * terrainFreq) * heightMultiplier + heightAddition;
            if (height > yMax)
            {
                height = yMax;
            }
            //print("Height: " + height.ToString());
            for (int y = 0; y < height; y++)
            {
                PlaceTile(x, y, placeholderTile);
                //print("Placing tile");
            }
        }
    }

    void ColorTerrain()
    {
        // Have to replace the placeholder tiles that GenerateTerrain and SmoothBiomeBorders create
        for (int x = 0; x < xMax; x++)
        {
            Biome biome = GetBiome(GetChunk(x, 0));
            bool grass = false; // we haven't done grass yet
            int soil = 0; // no soil so far

            for (int y = yMax; y >= 0; y--)
            {
                // heading top down this time

                if (GetTile(x, y) != null && !grass)
                {
                    PlaceTile(x, y, biome.grassTile);
                    grass = true;
                }
                else if (GetTile(x, y) != null && soil < biome.dirtHeight)
                {
                    PlaceTile(x, y, biome.dirtTile);
                    soil++;
                }
                else if (GetTile(x, y) != null)
                {
                    PlaceTile(x, y, biome.stoneTile);
                }
            }
        }
    }

    void PlaceBackgroundTiles()
    {
        // For every tile, place a background tile of the same type
        for (int x = 0; x < xMax; x++)
            for (int y = 0; y < yMax; y++)
            {
                if (GetTile(x, y) != null)
                {
                    PlaceTile(x, y, GetTile(x, y), true);
                }
            }
    }
    void GenerateOres()
    {
        // Generate ores for each chunk
        for (int x = 0; x < xMax; x++)
        {
            Biome biome = GetBiome(GetChunk(x, 0));

            foreach (Ore ore in biome.ores)
            {
                for (int y = 0; y < yMax; y++)
                    if (ore.spreadTexture.GetPixel(x, y).r > 0.5f && y <= ore.maxSpawnHeight)
                    {
                        PlaceTile(x, y, ore.ore);
                    }
            }
        }
    }

    void CarveCaves()
    {
        // Remove foreground tiles according to a perlin noise texture
        // TODO: fix this up, maybe using a different generation method.
        Texture2D caveNoiseTexture = new Texture2D(xMax, yMax);

        GenerateNoiseTexture(caveFreq, caveCutoff, caveNoiseTexture);

        for (int x = 0; x < xMax; x++)
        {
            Biome biome = GetBiome(GetChunk(x, 0));

            if (biome.generateCaves)
            {
                for (int y = 0; y < yMax; y++)
                    if (caveNoiseTexture.GetPixel(x, y).r < 0.5) { PlaceTile(x, y, null); }
            }
        }
    }
    */

    public void GenerateChunk(int x, int y){
        if (GetChunk(x,y) != null){
            return;
        }
        
        // First, create the chunk

        GameObject chunk = new GameObject();
        chunk.name = "chunk_" + x.ToString() + "_" + y.ToString();
        chunk.transform.parent = transform;
        chunk.isStatic = true;
        chunk.AddComponent<TagHandler>();

        GameObject fg = new GameObject { name = "fg" };
        fg.AddComponent<Tilemap>();
        fg.AddComponent<TilemapRenderer>();
        fg.GetComponent<TilemapRenderer>().sortingLayerName = "Tiles-FG";
        fg.AddComponent<TilemapCollider2D>();
        fg.isStatic = true;
        fg.transform.parent = chunk.transform;

        GameObject bg = new GameObject { name = "bg" };
        bg.AddComponent<Tilemap>();
        bg.AddComponent<TilemapRenderer>();
        bg.GetComponent<TilemapRenderer>().sortingLayerName = "Tiles-BG";
        bg.isStatic = true;
        bg.GetComponent<Tilemap>().color = new Color(0.5f, 0.5f, 0.5f); // temporary, dims the background a bit
        bg.transform.parent = chunk.transform;

        chunks[x,y] = chunk;

        // What biome should this chunk be?
        Biome[] availableBiomes = new Biome[]{
            GetBiome(GetChunk(x+world.chunkSize,y+world.chunkSize)),
            GetBiome(GetChunk(x+world.chunkSize,y-world.chunkSize)),
            GetBiome(GetChunk(x-world.chunkSize,y+world.chunkSize)),
            GetBiome(GetChunk(x-world.chunkSize,y-world.chunkSize)),
            world.biomes[Random.Range(0, world.biomes.GetLength(0))]
        };

        int index = Random.Range(0,availableBiomes.Length);
        while (availableBiomes[index] == null){ // We can't have it be null, it might be null if the other chunks don't exist
            index = Random.Range(0,availableBiomes.Length);
        }

        chunk.GetComponent<TagHandler>().tags.Add(availableBiomes[index].name);


        // Now that we have the chunk, and know the biome, fill it with tiles

        

    }

}
