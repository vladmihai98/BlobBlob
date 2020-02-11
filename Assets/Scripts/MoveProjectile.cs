using UnityEngine;

public class MoveProjectile : MonoBehaviour
{
    [SerializeField] float speed;

    void Update()
    {
        if(speed >= 0)
        {
            transform.position += transform.forward * (speed * Time.deltaTime);
        }
    }
}
