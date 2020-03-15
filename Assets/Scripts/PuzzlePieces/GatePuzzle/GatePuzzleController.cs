using System.Collections;
using UnityEngine;

public class GatePuzzleController : MonoBehaviour
{
    [SerializeField] Transform gate;
    [SerializeField] CameraMovement camera;
    [SerializeField] GameObject successScreen;

    [Header("References to pieces of the puzzle.")]
    [SerializeField] SuspendedCrateController suspendedCrate;
    [SerializeField] FirstNumberController firstNumber;
    [SerializeField] MidCrateController midCrate;
    [SerializeField] RightCrateController rightCrate;
    [SerializeField] DarkCrateController darkCrate;

    private bool isPuzzleSolved = false;
    private Vector3 newPosition;

    private void Start()
    {
        newPosition = new Vector3(gate.position.x, gate.position.y + 50f, gate.position.z);
    }

    void Update()
    {
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
        camera.enabled = false;
        Vector3 newCameraPosition = newPosition;
        newCameraPosition.z -= 40f;
        newCameraPosition.y += 20f;
        camera.transform.position = Vector3.Lerp(transform.position, newCameraPosition, 0.1f);
        gate.position = Vector3.Lerp(gate.position, newPosition, 0.005f);
        StartCoroutine(DisplaySuccessMessage());
    }

    IEnumerator DisplaySuccessMessage()
    {
        yield return new WaitForSeconds(2f);
        Time.timeScale = 0f;
        successScreen.SetActive(true);
    }
}
