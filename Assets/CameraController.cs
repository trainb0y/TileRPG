using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public Vector2 offset;

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = new Vector3(target.position.x + offset.x, target.position.y + offset.y, transform.position.z);
    }
}
