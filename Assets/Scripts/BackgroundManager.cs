using UnityEngine;

public class BackgroundManager : MonoBehaviour
// A modified and expanded version of
// https://www.youtube.com/watch?v=wBol2xzxCOU
{
    public TerrainHandler terrain;
    public Transform playerTransform;


    private Transform cameraTransform;
    private Vector3 lastCameraPosition;

    private Background[] backgrounds;
    private GameObject[] backgroundObjects;

    void Start()
    {
        backgrounds = new Background[0];
        backgroundObjects = new GameObject[0];

        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;
        InvokeRepeating("SwitchBackground", 1f, 1);
        SwitchBackground();
    }

    /*
     Creates an array of GameObjects with SpriteRenderers, at the same index
     as the corresponding Background. They move in accordance with the Backrgound's parameters.

    Probably a dumb way to go about it...
     */




    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
        lastCameraPosition = cameraTransform.position;

        for(int i = 0; i < backgrounds.Length; i++)
        {
            Background background = backgrounds[i];
            GameObject backgroundObj = backgroundObjects[i];

            backgroundObj.transform.position += new Vector3(deltaMovement.x * background.parralaxMultiplier.x, deltaMovement.y * background.parralaxMultiplier.y, 0);



            Texture2D texture = background.sprite.texture;
            float textureUnitSizeX = texture.width / background.sprite.pixelsPerUnit;
            float textureUnitSizeY = texture.height / background.sprite.pixelsPerUnit;
            if (background.infiniteX)
            {
                if (Mathf.Abs(cameraTransform.position.x - transform.position.x) >= textureUnitSizeX)
                {
                    float offsetPositionX = (cameraTransform.position.x - backgroundObj.transform.position.x) % textureUnitSizeX;
                    backgroundObj.transform.position = new Vector3(cameraTransform.position.x + offsetPositionX, transform.position.y);
                }
            }
            if (background.infiniteY)
            {
                if (Mathf.Abs(cameraTransform.position.y - transform.position.y) >= textureUnitSizeY)
                {
                    float offsetPositionY = (cameraTransform.position.y - backgroundObj.transform.position.y) % textureUnitSizeY;
                    backgroundObj.transform.position = new Vector3(transform.position.x, cameraTransform.position.y + offsetPositionY);
                }
            }
        }
    }

    void SwitchBackground()
    {
        // Check for the current biome background, and if it's different,
        // fade the new one in

        // TEMPORARY
        // TODO: actually fade stuff
        
        if (terrain.GetBiome(terrain.GetChunk((int)playerTransform.position.x, (int)playerTransform.position.y)).backgrounds != backgrounds)
        {
            foreach (GameObject obj in backgroundObjects)
            {
                Destroy(obj);
            }

            backgrounds = terrain.GetBiome(terrain.GetChunk((int)playerTransform.position.x, (int)playerTransform.position.y)).backgrounds;
            backgroundObjects = new GameObject[backgrounds.Length];
            int layerOrder = 0;

            foreach (Background background in backgrounds)
            {
                GameObject bgObj = Instantiate(new GameObject());
                bgObj.transform.parent = transform.parent;
                bgObj.name = "background_" + background.name;
                SpriteRenderer renderer = bgObj.AddComponent<SpriteRenderer>();
                
                renderer.sortingLayerName = "Backgrounds";
                renderer.sortingOrder = layerOrder;
                renderer.sprite = background.sprite;
                bgObj.transform.position += new Vector3(background.offset.x, background.offset.y, 0);
                backgroundObjects[Mathf.Abs(layerOrder)] = bgObj;

                layerOrder -= 1;
            }
        }
    }
}
