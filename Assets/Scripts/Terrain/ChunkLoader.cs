using UnityEngine;

public class ChunkLoader : MonoBehaviour
{
    public int chunkPadding;

    public int loadPadding; // amount of chunks out to deactivate
    public TerrainHandler terrain;
    void Start()
    {
        InvokeRepeating("LoadChunks", 1f, 0.5f);  //1s delay, repeat every 0.5s
    }

    void SetChunk(int x, int y, bool active){
        // Set a chunk to a certain state, generate it if it doesn't exist
        GameObject chunk = terrain.GetChunkObject(x,y,true);
        if (chunk != null){
            chunk.SetActive(active);
        }
        else{
            terrain.GenerateChunk(x,y);
            terrain.GetChunkObject(x,y,true).SetActive(active);
        }
    }

    void LoadChunks()
    {
        Vector3 cameraPos = Camera.main.transform.position;
        float height = 2f * Camera  .main.orthographicSize;
        float width = height * Camera.main.aspect;

        int xMin = (int)(cameraPos.x - (width / 2) - (terrain.world.chunkSize * chunkPadding));
        int xMax = (int)(cameraPos.x + (width / 2) + (terrain.world.chunkSize * chunkPadding));

        int yMin = (int)(cameraPos.y - (height / 2) - (terrain.world.chunkSize * chunkPadding));
        int yMax = (int)(cameraPos.y + (height / 2) + (terrain.world.chunkSize * chunkPadding));
        

        for (int x = xMin - (terrain.world.chunkSize * loadPadding); x <= xMax + (terrain.world.chunkSize * loadPadding); x += terrain.world.chunkSize){
            for (int y = yMin - (terrain.world.chunkSize * loadPadding); y <= yMax + (terrain.world.chunkSize * loadPadding); y += terrain.world.chunkSize){
                if (xMin <= x && x <= xMax && yMin <= y && y <= yMax){SetChunk(x,y,true);}
                else{SetChunk(x,y,false);}
            }
        }
    }
}
