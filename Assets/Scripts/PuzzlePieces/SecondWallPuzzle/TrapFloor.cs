using UnityEngine;

public class TrapFloor : MonoBehaviour
{
    [SerializeField] Transform trapDoor;
    [SerializeField] Vector3 trapOffset;
    [SerializeField] Transform ceilingCrateLeft;
    [SerializeField] Transform ceilingCrateRight;

    private GameController gameController;

    void Start()
    {
        gameController = FindObjectOfType<GameController>();    
    }

    void OnCollisionEnter(Collision collision)
    {
        // Close the door to trap Monty.
        trapDoor.position += trapOffset;

        // If the crates exist, prevent Monty from acting by faking his death.
        if(ceilingCrateLeft && ceilingCrateRight)
        {
            gameController.FakeMontyNotAlive(false);
        }

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
