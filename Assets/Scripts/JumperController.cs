using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperController : MonoBehaviour
{
    //TODO remove multiple jump; isGrounded?

    [SerializeField] float movementSpeed = 5;
    [SerializeField] float jumpHeight = 300;

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
        Jump();
    }

    void ProcessInput()
    {
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
        {
            velocity = Vector3.forward + Vector3.left;
        }
        else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
        {
            velocity = Vector3.forward + Vector3.right;
        }
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
        {
            velocity = Vector3.back + Vector3.left;
        }
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
        {
            velocity = Vector3.back + Vector3.right;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            velocity = Vector3.forward;
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            velocity = Vector3.zero;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            velocity = Vector3.left;
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            velocity = Vector3.zero;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            velocity = Vector3.back;
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            velocity = Vector3.zero;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            velocity = Vector3.right;
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            velocity = Vector3.zero;
        }
    }

    void Move()
    {
        rigidbody.MovePosition(transform.position + (velocity * movementSpeed * Time.deltaTime));
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rigidbody.AddForce(0, jumpHeight, 0);
        }
    }
}
