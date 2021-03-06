﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [Header("Game controls")]
    [SerializeField] CameraMovement camera;
    [SerializeField] GatePuzzleController gatePuzzle;
    [SerializeField] MontyController monty;
    [SerializeField] SeeSharpController seeSharp;

    [Header("Game windows")]
    [SerializeField] GameObject pauseCanvas;
    [SerializeField] GameObject gameOverCanvas;

    private bool isMontyAlive;
    private bool isSeeSharpAlive;

    public void FakeMontyNotAlive(bool isAlive)
    {
        isMontyAlive = isAlive;
    }

    public bool IsMontyAlive() { return isMontyAlive; }

    public bool IsSeeSharpAlive() { return isSeeSharpAlive; }

    public void ResumeGame() { Time.timeScale = 1f; }

    public void ReturnToMenu() { SceneManager.LoadScene(0); }

    public void TryAgain() { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }

    public void QuitGame() { Application.Quit(); }

    void Start()
    {
        isMontyAlive = true;
        isSeeSharpAlive = true;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.P))
        {
            PauseGame();
        }

        if(Input.GetKey(KeyCode.Alpha0))
        {
            AssumeGatePuzzleIsComplete();
        }

        HackButtons();

        if(isMontyAlive && monty.GetCurrentHealth() <= 0)
        {
            isMontyAlive = false;
        }

        if (isSeeSharpAlive && seeSharp.GetCurrentHealth() <= 0)
        {
            isSeeSharpAlive = false;
        }

        if(!isMontyAlive && !isSeeSharpAlive)
        {
            DisplayGameOver();
        }
    }

    void PauseGame()
    {
        // Pause game.
        Time.timeScale = 0f;
        pauseCanvas.SetActive(true);
    }

    void AssumeGatePuzzleIsComplete()
    {
        gatePuzzle.ForcePuzzleCompletion();
    }

    void HackButtons()
    {
        // Full Heal and Mana
        if (Input.GetKey(KeyCode.F))
        {
            if (isSeeSharpAlive)
            {
                seeSharp.UseHeal(500);
            }

            if (isMontyAlive)
            {
                monty.UseHeal(500);
                monty.GainMana(1000);
            }
        }

        // Teleport to first point.
        if (Input.GetKey(KeyCode.Alpha1))
        {
            if (isSeeSharpAlive)
            {
                Vector3 newPos = new Vector3(8.6f, 0f, 27f);
                seeSharp.transform.position = newPos;
            }

            if (isMontyAlive)
            {
                Vector3 newPos = new Vector3(-10f, 0f, 27f);
                monty.transform.position = newPos;
            }
        }

        // Teleport in front of first gate puzzle.
        if (Input.GetKey(KeyCode.Alpha2))
        {
            if (isSeeSharpAlive)
            {
                Vector3 newPos = new Vector3(-13f, 0f, 556f);
                seeSharp.transform.position = newPos;
            }

            if (isMontyAlive)
            {
                Vector3 newPos = new Vector3(-26f, 0f, 556f);
                monty.transform.position = newPos;
            }
        }

        // Teleport in front of trap gate puzzle.
        if (Input.GetKey(KeyCode.Alpha3))
        {
            if (isSeeSharpAlive)
            {
                Vector3 newPos = new Vector3(-260f, 0f, 657f);
                seeSharp.transform.position = newPos;
            }

            if (isMontyAlive)
            {
                Vector3 newPos = new Vector3(-275f, 0f, 657f);
                monty.transform.position = newPos;
            }
        }

        // Teleport in front of final gate puzzle.
        if (Input.GetKey(KeyCode.Alpha4))
        {
            if (isSeeSharpAlive)
            {
                Vector3 newPos = new Vector3(-123f, 0f, 1637f);
                seeSharp.transform.position = newPos;
            }

            if (isMontyAlive)
            {
                Vector3 newPos = new Vector3(-140, 0f, 1637f);
                monty.transform.position = newPos;
            }
        }

        ManipulateCamera();
    }

    /// <summary>
    /// YGHJ for movement and TU for raise/lower
    /// </summary>
    void ManipulateCamera()
    {
        // Move the camera forward.
        if (Input.GetKey(KeyCode.Y))
        {
            camera.enabled = false;
            camera.transform.position += Vector3.forward * 20f * Time.deltaTime;
        }

        // Move the camera to the right.
        if (Input.GetKey(KeyCode.J))
        {
            camera.enabled = false;
            camera.transform.position += Vector3.right * 20f * Time.deltaTime;
        }

        // Move the camera back.
        if (Input.GetKey(KeyCode.H))
        {
            camera.enabled = false;
            camera.transform.position += Vector3.back * 20f * Time.deltaTime;
        }

        // Move the camera to the left.
        if (Input.GetKey(KeyCode.G))
        {
            camera.enabled = false;
            camera.transform.position += Vector3.left * 20f * Time.deltaTime;
        }

        // Raise the camera.
        if (Input.GetKey(KeyCode.U))
        {
            camera.enabled = false;
            camera.transform.position += Vector3.up * 20f * Time.deltaTime;
        }

        // Lower the camera.
        if (Input.GetKey(KeyCode.T))
        {
            camera.enabled = false;
            camera.transform.position += Vector3.down * 20f * Time.deltaTime;
        }

        // Reset camera
        if (Input.GetKey(KeyCode.I))
        {
            camera.enabled = true;
        }
    }

    void DisplayGameOver()
    {
        gameOverCanvas.SetActive(true);
    }
}
