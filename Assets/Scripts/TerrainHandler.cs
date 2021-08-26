using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainHandler : MonoBehaviour
{
    public int xMax = 100;
    public int yMax = 100;

    public int chunkSize = 10;

    private GameObject[] chunks;

    // Start is called before the first frame update
    void Start()
    {
        CreateChunks();
    }

    void CreateChunks()
    {
        int numChunks = xMax / chunkSize;
        chunks = new GameObject[numChunks];
        for (int i = 0; i < numChunks; i++)
        {
            GameObject chunk = new GameObject();
            chunk.name = i.ToString();
            chunk.transform.parent = this.transform;
            chunks[i] = chunk;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
