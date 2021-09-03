using UnityEngine;

public class BackgroundScript : MonoBehaviour
// A modified version of
// https://www.youtube.com/watch?v=wBol2xzxCOU
{
    public bool infiniteX;
    public bool infiniteY;

    public Vector2 paralaxEffectMultiplier;
    
    private Transform cameraTransform;
    private Vector3 lastCameraPosition;
    private float textureUnitSizeX;
    private float textureUnitSizeY;

    void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;
        textureUnitSizeX = texture.width / sprite.pixelsPerUnit;
        textureUnitSizeY = texture.height / sprite.pixelsPerUnit;
    }



    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
        transform.position += new Vector3(deltaMovement.x * paralaxEffectMultiplier.x, deltaMovement.y * paralaxEffectMultiplier.y, 0);
        lastCameraPosition = cameraTransform.position;

        if (infiniteX)
        {
            if (Mathf.Abs(cameraTransform.position.x - transform.position.x) >= textureUnitSizeX)
            {
                float offsetPositionX = (cameraTransform.position.x - transform.position.x) % textureUnitSizeX;
                Debug.Log(offsetPositionX);
                transform.position = new Vector3(cameraTransform.position.x + offsetPositionX, transform.position.y, 0);
            }
        }
        if (infiniteY)
        {
            if (Mathf.Abs(cameraTransform.position.y - transform.position.y) >= textureUnitSizeY)
            {
                float offsetPositionY = (cameraTransform.position.y - transform.position.y) % textureUnitSizeY;
                transform.position = new Vector3(transform.position.x, cameraTransform.position.y + offsetPositionY);
            }
        }
    }
}
