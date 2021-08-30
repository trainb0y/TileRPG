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
        
        for (int x = 0; x < terrain.xMax; x += terrain.chunkSize)
        {
            for (int y = 0; y < terrain.yMax; y += terrain.chunkSize)
            {
                if ((x > cameraPos.x - (width / 2) - (terrain.chunkSize * chunkPadding) && x < cameraPos.x + (width / 2) + (terrain.chunkSize * chunkPadding)) &&
                    (y > cameraPos.y - (height / 2) - (terrain.chunkSize * chunkPadding) && y < cameraPos.y + (height / 2) + (terrain.chunkSize * chunkPadding)))
                {
                    terrain.GetChunk(x, y).SetActive(true);
                }
                else
                {
                    terrain.GetChunk(x, y).SetActive(false);
                }
                
            }
        }
    }
}
