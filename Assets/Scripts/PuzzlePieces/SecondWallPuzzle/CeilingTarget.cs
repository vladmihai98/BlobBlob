using UnityEngine;
using UnityEngine.UI;

public class CeilingTarget : MonoBehaviour
{
    [SerializeField] Canvas hud;
    [SerializeField] Image healthBar;
    [SerializeField] Transform dragonToSpawn;
    [SerializeField] Transform boxToDestroy;

    private bool isHudRevealed;
    private bool puzzleSolved;
    private int healthPoints;

    void Start()
    {
        isHudRevealed = false;
        puzzleSolved = false;
        healthPoints = 5;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Bullet"))
        {
            if(!isHudRevealed)
            {
                isHudRevealed = true;
                hud.gameObject.SetActive(true);
                healthPoints--;
                healthBar.fillAmount = (float) healthPoints / 5;
                dragonToSpawn.gameObject.SetActive(true);
            }

            if(isHudRevealed)
            {
                if(healthPoints > 0)
                {
                    healthPoints--;
                    healthBar.fillAmount = (float) healthPoints / 5;
                }

                if(healthPoints == 0)
                {
                    Destroy(boxToDestroy.gameObject);

                    // Decrease to avoid repeating.
                    healthPoints--;

                    puzzleSolved = true;
                }
            }
        }
    }

    public bool IsSolved()
    {
        return puzzleSolved;
    }
}
