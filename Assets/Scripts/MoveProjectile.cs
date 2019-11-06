using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveProjectile : MonoBehaviour
{
    [SerializeField] float speed;

    // Update is called once per frame
    void Update()
    {
        if(speed >= 0)
        {
            transform.position += transform.forward * (speed * Time.deltaTime);
        }
    }
}
