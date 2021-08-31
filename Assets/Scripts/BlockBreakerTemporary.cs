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
        if (Input.GetMouseButton(0))
        {
            terrain.PlaceTile((int)mainCamera.ScreenToWorldPoint(Input.mousePosition).x, (int)mainCamera.ScreenToWorldPoint(Input.mousePosition).y, null, Input.GetKey(KeyCode.B)); ;
        }
        if (Input.GetMouseButton(1))
        {
            terrain.PlaceTile((int)mainCamera.ScreenToWorldPoint(Input.mousePosition).x, (int)mainCamera.ScreenToWorldPoint(Input.mousePosition).y, tile, Input.GetKey(KeyCode.B), true);
        }

    }
}


