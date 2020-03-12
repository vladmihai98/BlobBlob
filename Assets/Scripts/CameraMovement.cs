using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] Transform monty;
    [SerializeField] Transform seeSharp;
    [SerializeField] Vector3 positionOffset;
    [SerializeField] float zoomInZ;
    [SerializeField] float zoomOutZ;

    private Camera camera;
    private Bounds bounds;

    void Start()
    {
        camera = GetComponent<Camera>();
    }

    void Update()
    {
        bounds = GetBounds();
        PositionCamera();
        AdjustZoom();
    }

    Bounds GetBounds()
    {
        Bounds bounds = new Bounds(monty.position, Vector3.zero);
        bounds.Encapsulate(seeSharp.position);
        return bounds;
    }

    void PositionCamera()
    {
        Vector3 newPosition = bounds.center + positionOffset;
        transform.position = Vector3.Lerp(transform.position, newPosition, 0.5f);
    }

    void AdjustZoom()
    {
        float newZoom = Mathf.Lerp(zoomInZ, zoomOutZ, bounds.size.x / 58f);
        Vector3 newCameraPosition = new Vector3
        {
            x = camera.transform.position.x,
            y = camera.transform.position.y,
            z = newZoom
        };
        camera.transform.position = Vector3.Lerp(camera.transform.position, newCameraPosition, 0.1f);
    }
}