using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PusherController : MonoBehaviour
{
    [SerializeField] float movementSpeed = 5;

    private Rigidbody rigidbody;
    private Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();
        Move();
    }

    void ProcessInput()
    {
        if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.LeftArrow))
        {
            velocity = Vector3.forward + Vector3.left;
        }
        else if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.RightArrow))
        {
            velocity = Vector3.forward + Vector3.right;
        }
        else if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftArrow))
        {
            velocity = Vector3.back + Vector3.left;
        }
        else if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.RightArrow))
        {
            velocity = Vector3.back + Vector3.right;
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            velocity = Vector3.forward;
        }
        else if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            velocity = Vector3.zero;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            velocity = Vector3.left;
        }
        else if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            velocity = Vector3.zero;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            velocity = Vector3.back;
        }
        else if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            velocity = Vector3.zero;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            velocity = Vector3.right;
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            velocity = Vector3.zero;
        }
    }

    void Move()
    {
        rigidbody.MovePosition(transform.position + (velocity * movementSpeed * Time.deltaTime));
    }
}
