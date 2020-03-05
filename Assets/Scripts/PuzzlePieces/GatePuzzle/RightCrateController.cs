using System;
using UnityEngine;

public class RightCrateController : MonoBehaviour
{
    private bool collidedWithBotCollider = false;
    private bool collidedWithMidCrate = false;
    private bool isPuzzleSolved = false;
    private Rigidbody rigidbody;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.name.Equals("BotCollider"))
        {
            collidedWithBotCollider = true;
        }

        if(collision.transform.name.Equals("MidCrate"))
        {
            collidedWithMidCrate = true;
        }

        if(collidedWithMidCrate && collidedWithBotCollider)
        {
            FreezePosition();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.name.Equals("BotCollider"))
        {
            collidedWithBotCollider = false;
        }

        if (collision.transform.name.Equals("MidCrate"))
        {
            collidedWithMidCrate = false;
        }
    }

    private void FreezePosition()
    {
        rigidbody.constraints = RigidbodyConstraints.FreezePosition;
        print("yeeee 444");
        isPuzzleSolved = true;
    }

    public bool GetPuzzleStatus()
    {
        return isPuzzleSolved;
    }
}
