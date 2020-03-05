using UnityEngine;

public class SuspendedCrateController : MonoBehaviour
{
    private bool duckedOnce;
    private bool stompedTwice;
    private bool isPuzzleSolved = false;
    private int duck = 0;
    private int stomp = 0;

    void Update()
    {
        if(duckedOnce && stompedTwice)
        {
            print("Hell yeah. 11");
            isPuzzleSolved = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        duck++;
        duckedOnce = duck >= 1;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.name.Equals("Monty"))
        {
            stomp++;

            stompedTwice = stomp >= 2;
        }
    }

    public bool GetPuzzleStatus()
    {
        return isPuzzleSolved;
    }
}
