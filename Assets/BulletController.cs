using UnityEngine;

public class BulletController : MonoBehaviour
{
    private Vector3 direction;

    public Vector3 GetDirection()
    {
        return direction;
    }

    public void SetDirection(Vector3 newDirection)
    {
        direction = newDirection;
    }
}
