using UnityEngine;
using UnityEngine.UI;

public class LeverController : MonoBehaviour
{
    [Tooltip("Reference to a target that the player has to shoot.")]
    [SerializeField] Image shootingTarget;

    [SerializeField] Transform wallToMove;
    [SerializeField] [Tooltip("Offset for the wall movement.")] Vector3 offset;

    private bool rotateLever = false;
    private bool moveWall = false;

    void Update()
    {
        if(rotateLever)
        {
            RotateLever();
        }

        if(moveWall)
        {
            MoveWall();
        }
    }

    void RotateLever()
    {
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0f, 0f, 45f), 0.1f);
    }

    void OnTriggerEnter(Collider other)
    {
        rotateLever = true;
        if(shootingTarget)
        {
            EnableShootingTarget();
        }

        if(wallToMove)
        {
            moveWall = true;
        }
    }

    void EnableShootingTarget()
    {
        shootingTarget.gameObject.SetActive(true);
        shootingTarget.GetComponent<BoxCollider>().enabled = true;
    }

    void MoveWall()
    {
        Vector3 newPosition = wallToMove.position + offset;
        wallToMove.position = Vector3.Lerp(wallToMove.position, newPosition, 0.005f);

        if(wallToMove.position == newPosition)
        {
            wallToMove.gameObject.SetActive(false);
        }
    }
}
