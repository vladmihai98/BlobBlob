using UnityEngine;

public class TrapFloor : MonoBehaviour
{
    [SerializeField] Transform trapDoor;
    [SerializeField] Vector3 trapOffset;
    [SerializeField] Transform ceilingCrateLeft;
    [SerializeField] Transform ceilingCrateRight;

    void OnCollisionEnter(Collision collision)
    {
        // Close the door to trap Monty.
        trapDoor.position += trapOffset;

        // Reveal the next piece of the puzzle.
        if(ceilingCrateLeft)
        {
            ceilingCrateLeft.gameObject.SetActive(true);
        }
        if(ceilingCrateRight)
        {
            ceilingCrateRight.gameObject.SetActive(true);
        }
    }
}
