using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public Vector2 offset;
    public float smoothTime = 0.3f;
    private Vector3 velocity = Vector3.zero;

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 desiredPosition = new Vector3(player.position.x + offset.x, player.position.y + offset.y, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
    }
}

