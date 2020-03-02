using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    [SerializeField] bool ok;

    void Start()
    {
        
    }

    void Update()
    {
        MoveObstacle();
    }

    void MoveObstacle()
    {
        if(ok)
        {
            Vector3 newPosition = new Vector3(transform.position.x + 80f, transform.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, newPosition, 0.001f);
        }
    }
}
