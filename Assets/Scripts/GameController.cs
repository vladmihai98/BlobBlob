using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] MontyController monty;
    [SerializeField] SeeSharpController seeSharp;

    private bool isMontyAlive;
    private bool isSeeSharpAlive;

    public bool IsMontyAlive() { return isMontyAlive; }

    public bool IsSeeSharpAlive() { return isSeeSharpAlive; }

    void Start()
    {
        isMontyAlive = true;
        isSeeSharpAlive = true;
    }

    void Update()
    {
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

    void DisplayGameOver()
    {

    }
}
