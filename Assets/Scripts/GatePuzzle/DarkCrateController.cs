using System.Collections;
using UnityEngine;

public class DarkCrateController : MonoBehaviour
{
    private bool montyTriggered = false;
    private bool seeSharpCollided = false;
    private bool isPuzzleSolved = false;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.name.Equals("SeeSharp"))
        {
            seeSharpCollided = true;
        }

        if (!montyTriggered)
        {
            StartCoroutine(CheckConcurrency());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.name.Equals("Monty"))
        {
            montyTriggered = true;
        }

        if (!seeSharpCollided)
        {
            StartCoroutine(CheckConcurrency());
        }
    }

    IEnumerator CheckConcurrency()
    {
        if(montyTriggered && seeSharpCollided)
        {
            print("cool 1");
            isPuzzleSolved = true;
        }

        yield return new WaitForSecondsRealtime(0.5f);
        if (montyTriggered && seeSharpCollided)
        {
            print("cool 2");
            isPuzzleSolved = true;
        }
        else
        {
            montyTriggered = false;
            seeSharpCollided = false;
        }
    }

    public bool GetPuzzleStatus()
    {
        return isPuzzleSolved;
    }
}
