using UnityEngine;

public class FirstNumberController : MonoBehaviour
{
    private bool isMontyOnTop = false;
    private bool isPuzzleSolved = false;
    private Rigidbody rigidbody;
    Vector3 initPosition;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        initPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.name.Equals("Monty"))
        {
            isMontyOnTop = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        isMontyOnTop = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.name.Equals("FixedCrate"))
        {
            if(!isMontyOnTop)
            {
                transform.position = Vector3.Lerp(transform.position, initPosition, 0.1f);
            }
            else
            {
                FreezePosition();
            }
        }

    }

    void FreezePosition()
    {
        rigidbody.constraints = RigidbodyConstraints.FreezePosition;
        print("Yay 22");
        isPuzzleSolved = true;
    }

    public bool GetPuzzleStatus()
    {
        return isPuzzleSolved;
    }
}
