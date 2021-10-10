using UnityEngine;

public class ChunkLoader : MonoBehaviour
{
    public int chunkPadding;
    public TerrainHandler terrain;
    void Start()
    {
        InvokeRepeating("LoadChunks", 1f, 0.5f);  //1s delay, repeat every 0.5s
    }
    void LoadChunks()
    {
        Vector3 cameraPos = Camera.main.transform.position;
        float height = 2f * Camera.main.orthographicSize;
        float width = height * Camera.main.aspect;

        int xMin = (int)(cameraPos.x - (width / 2) - (terrain.world.chunkSize * chunkPadding));
        int xMax = (int)(cameraPos.x + (width / 2) + (terrain.world.chunkSize * chunkPadding));

        int yMin = (int)(cameraPos.y - (height / 2) - (terrain.world.chunkSize * chunkPadding));
        int yMax = (int)(cameraPos.y + (height / 2) + (terrain.world.chunkSize * chunkPadding));
        
        for (int x = xMin; x <= xMax; x += terrain.world.chunkSize){
            for (int y = yMin; y <= yMax; y += terrain.world.chunkSize){
                GameObject chunk = terrain.GetChunkObject(x,y,true);
                if (chunk != null){
                    chunk.SetActive(true);
                }
                else{
                    terrain.GenerateChunk(x,y);
                    terrain.GetChunkObject(x,y,true).SetActive(true);
                }
            }
        }
    }
}
