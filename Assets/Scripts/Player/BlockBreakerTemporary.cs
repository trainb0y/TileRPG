using UnityEngine;
using UnityEngine.Tilemaps;

public class BlockBreakerTemporary : MonoBehaviour
{
    public TerrainHandler terrain;
    public Camera mainCamera;
    public Tile tile;
    // Update is called once per frame
    void Update()
    {
        Vector2 pos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButton(0))
        {
            terrain.PlaceTile((int)pos.x, (int)pos.y, null, Input.GetKey(KeyCode.B));
        }
        if (Input.GetMouseButton(1))
        {
            terrain.PlaceTile((int)pos.x, (int)pos.y, tile, Input.GetKey(KeyCode.B));
        }

    }
}


