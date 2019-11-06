using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraZoom : MonoBehaviour
{
    //TODO change the fieldView to zoom since it distorts the characters
    // make it so that it zooms out based on the size of the X instead of the field view change.

    [SerializeField] List<Transform> targets;
    [SerializeField] Vector3 offset;
    [SerializeField] float smoothTime = 0.5f;
    [SerializeField] float zoomedOutZPos = -14f;
    [SerializeField] float zoomedInZPos = -7f;
    [SerializeField] float zoomLimiter = 16f;

    private Vector3 velocity;
    private Camera camera;

    void Start()
    {
        camera = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if(targets.Count == 0)
        {
            return;
        }
        Bounds bounds = GetBounds();
        MoveCamera(bounds);
        AdjustZoom(bounds);
    }

    private void MoveCamera(Bounds bounds)
    {
        Vector3 centerPoint = bounds.center;

        Vector3 newPosition = centerPoint + offset;

        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
    }

    private void AdjustZoom(Bounds bounds)
    {
        float newZoom = Mathf.Lerp(zoomedInZPos, zoomedOutZPos, bounds.size.x / zoomLimiter);
        Vector3 newCameraPosition = new Vector3
        {
            x = camera.transform.position.x,
            y = camera.transform.position.y,
            z = Mathf.Lerp(camera.transform.position.z, newZoom, Time.deltaTime)
        };
        camera.transform.position = newCameraPosition;
    }

    private Bounds GetBounds()
    {
        Bounds bounds = new Bounds(targets[0].position, Vector3.zero);

        foreach(Transform target in targets)
        {
            bounds.Encapsulate(target.position);
        }

        return bounds;
    }
}
