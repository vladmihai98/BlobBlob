using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] Transform monty;
    [SerializeField] Transform seeSharp;
    [SerializeField] Vector3 positionOffset;

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
}