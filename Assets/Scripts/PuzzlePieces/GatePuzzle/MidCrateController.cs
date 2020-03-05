using UnityEngine;

public class MidCrateController : MonoBehaviour
{
    private bool collidedWithBotCollider = false;
    private bool collidedWithFixedCrate = false;

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

        if (collision.transform.name.Equals("FixedCrate"))
        {
            collidedWithFixedCrate = true;
        }

        if(collidedWithBotCollider && collidedWithFixedCrate)
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

        if (collision.transform.name.Equals("FixedCrate"))
        {
            collidedWithFixedCrate = false;
        }
    }

    void FreezePosition()
    {
        rigidbody.constraints = RigidbodyConstraints.FreezePosition;
        print("yaya 333");
        isPuzzleSolved = true;
    }

    public bool GetPuzzleStatus()
    {
        return isPuzzleSolved;
    }
}
