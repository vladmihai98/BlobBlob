using UnityEngine;

public class TrapLever : MonoBehaviour
{
    [SerializeField] Transform trapDoor;
    [SerializeField] Vector3 trapOffset;
    [SerializeField] Transform boxToDestroy;

    private bool rotateLever = false;
    private bool destroyBox = false;

    void Update()
    {
        if(rotateLever)
        {
            RotateLever();
        }

        if(destroyBox)
        {
            ActivateTrap();
        }
    }

    void RotateLever()
    {
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0f, 0f, 45f), 0.1f);

        if(transform.localRotation == Quaternion.Euler(0f, 0f, 45f))
        {
            destroyBox = true;
        }
    }

    void ActivateTrap()
    {
        // prevent a second iteration of this method.
        destroyBox = false;
        rotateLever = false;

        // Make the player fall under ground level.
        Destroy(boxToDestroy.gameObject);
        trapDoor.position += trapOffset;
    }

    void OnTriggerEnter(Collider other)
    {
        rotateLever = true;
    }
}
