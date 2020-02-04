using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToMouse : MonoBehaviour
{
    [SerializeField] Camera camera;
    [SerializeField] float maximumLength;

    private Ray mouseRay;
    private Vector3 position;
    private Vector3 direction;
    private Quaternion rotation;

    // TODO when pointing at objects far away the direction gets messed up; maxLength seems to do smth?
    void Update()
    {
        if(camera != null)
        {
            RaycastHit hit;
            var mousePosition = Input.mousePosition;
            mouseRay = camera.ScreenPointToRay(mousePosition);

            if(Physics.Raycast(mouseRay.origin, mouseRay.direction, out hit, maximumLength))
            {
                RotateToMouseDirection(gameObject, hit.point);
            }
            else
            {
                position = mouseRay.GetPoint(maximumLength);
                RotateToMouseDirection(gameObject, position);
            }
        }
    }

    void RotateToMouseDirection(GameObject obj, Vector3 destination)
    {
        direction = destination - obj.transform.position;
        rotation = Quaternion.LookRotation(direction);
        obj.transform.localRotation = Quaternion.Lerp(obj.transform.rotation, rotation, 1);
    }

    public Quaternion GetRotation()
    {
        return rotation;
    }
}
