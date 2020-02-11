using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    [SerializeField] float secondsAlive = 1f;

    void Start()
    {
        Destroy(gameObject, secondsAlive);
    }
}
