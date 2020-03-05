using UnityEngine;
using UnityEngine.UI;

public class TargetController : MonoBehaviour
{
    [SerializeField] Image healthImage;
    [SerializeField] Canvas hud;
    [SerializeField] Transform boxToDestroy;
    [SerializeField] Transform boxToReveal;

    private bool isHudRevealed;
    private int healthPoints;

    void Start()
    {
        isHudRevealed = false;
        healthPoints = 5;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Bullet"))
        {
            if(!isHudRevealed)
            {
                isHudRevealed = true;
                hud.gameObject.SetActive(true);
            }
            
            if(isHudRevealed)
            {
                if(healthPoints > 0)
                {
                    healthPoints--;
                    healthImage.fillAmount = (float) healthPoints / 5;
                }

                if(healthPoints == 0)
                {
                    hud.gameObject.SetActive(false);
                    gameObject.SetActive(false);
                    Destroy(boxToDestroy.gameObject);
                    boxToReveal.gameObject.SetActive(true);

                    // Decrease so that we don't repeat this.
                    healthPoints--;
                }
            }
        }
    }
}
