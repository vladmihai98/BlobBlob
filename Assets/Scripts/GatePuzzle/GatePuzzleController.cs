using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatePuzzleController : MonoBehaviour
{
    [SerializeField] Transform gate;

    [Header("References to pieces of the puzzle.")]
    [SerializeField] SuspendedCrateController suspendedCrate;
    [SerializeField] FirstNumberController firstNumber;
    [SerializeField] MidCrateController midCrate;
    [SerializeField] RightCrateController rightCrate;
    [SerializeField] DarkCrateController darkCrate;

    [SerializeField] bool test = false;

    private bool isPuzzleSolved = false;
    private Vector3 newPosition;

    private void Start()
    {
        newPosition = new Vector3(gate.position.x, gate.position.y + 50f, gate.position.z);
    }

    void Update()
    {
        if(test)
        {
            MoveGate();
        }

        if (!isPuzzleSolved)
        {
            if (suspendedCrate.GetPuzzleStatus() && firstNumber.GetPuzzleStatus() &&
               midCrate.GetPuzzleStatus() && rightCrate.GetPuzzleStatus() && darkCrate.GetPuzzleStatus())
            {
                isPuzzleSolved = true;
                MoveGate();
            }
        }
        else
        {
            MoveGate();
        }
    }

    void MoveGate()
    {
        gate.position = Vector3.Lerp(gate.position, newPosition, 0.005f);
    }
}
