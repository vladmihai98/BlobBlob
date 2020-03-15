using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] MontyController monty;
    [SerializeField] SeeSharpController seeSharp;

    private bool isMontyAlive;
    private bool isSeeSharpAlive;

    public void FakeMontyNotAlive(bool isAlive)
    {
        isMontyAlive = isAlive;
    }

    public bool IsMontyAlive() { return isMontyAlive; }

    public bool IsSeeSharpAlive() { return isSeeSharpAlive; }

    void Start()
    {
        isMontyAlive = true;
        isSeeSharpAlive = true;
    }

    void Update()
    {
        HackButton();

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

    void HackButton()
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
    }

    void DisplayGameOver()
    {

    }
}
