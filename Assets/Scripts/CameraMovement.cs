using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] Vector3 positionOffset;
    [SerializeField] float zoomInZ;
    [SerializeField] float zoomOutZ;
    [SerializeField] float maxBounds;

    private Camera camera;
    private Bounds bounds;
    private MontyController montyController;
    private SeeSharpController seeSharpController;
    private GameController gameController;

    void Start()
    {
        bounds = new Bounds();
        camera = GetComponent<Camera>();
        montyController = FindObjectOfType<MontyController>();
        seeSharpController = FindObjectOfType<SeeSharpController>();
        gameController = FindObjectOfType<GameController>();
    }

    void Update()
    {
        bounds = GetBounds();
        PositionCamera();

        // Adjust zoom only if both players are alive.
        if(gameController.IsMontyAlive() && gameController.IsSeeSharpAlive())
        {
            AdjustZoom();
        }
    }

    Bounds GetBounds()
    {
        if(gameController.IsMontyAlive())
        {
            bounds = new Bounds(montyController.transform.position, Vector3.zero);
            if(gameController.IsSeeSharpAlive())
            {
                bounds.Encapsulate(seeSharpController.transform.position);
            }
        }
        else if(gameController.IsSeeSharpAlive())
        {
            bounds = new Bounds(seeSharpController.transform.position, Vector3.zero);
        }
        return bounds;
    }

    void PositionCamera()
    {
        Vector3 newPosition = bounds.center + positionOffset;
        transform.position = Vector3.Lerp(transform.position, newPosition, 0.5f);
    }

    void AdjustZoom()
    {
        float newZoom = Mathf.Lerp(zoomInZ, zoomOutZ, bounds.size.x / maxBounds);
        Vector3 newCameraPosition = new Vector3
        {
            x = camera.transform.position.x,
            y = camera.transform.position.y,
            z = camera.transform.position.z + newZoom
        };
        camera.transform.position = Vector3.Lerp(camera.transform.position, newCameraPosition, 0.1f);
    }
}