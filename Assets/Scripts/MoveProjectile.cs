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

    //void OnCollisionEnter(Collision collision)
    //{
    //    print("who" + collision.gameObject + "at" + collision.transform.position);
    //}

    //private void OnTriggerEnter(Collider other)
    //{
    //    print("who" + other.gameObject + "at" + other.transform.position);
    //}
}
