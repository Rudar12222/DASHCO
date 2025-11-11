using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraTilemapBounds : MonoBehaviour
{
    public Transform target;          // Player
    public Tilemap tilemap;           // Your ground/wall tilemap

    private Vector3 minBounds;
    private Vector3 maxBounds;

    private float halfHeight;
    private float halfWidth;

    private void Start()
    {
        // Get the bounds of the Tilemap
        Bounds bounds = tilemap.localBounds;
        minBounds = bounds.min;
        maxBounds = bounds.max;

        Camera cam = Camera.main;
        halfHeight = cam.orthographicSize;
        halfWidth = halfHeight * cam.aspect;
    }

    private void LateUpdate()
    {
        // Clamp camera to tilemap bounds
        float clampedX = Mathf.Clamp(target.position.x, minBounds.x + halfWidth, maxBounds.x - halfWidth);
        float clampedY = Mathf.Clamp(target.position.y, minBounds.y + halfHeight, maxBounds.y - halfHeight);

        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }
}
