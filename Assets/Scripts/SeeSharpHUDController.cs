using UnityEngine;
using UnityEngine.UI;

public class SeeSharpHUDController : MonoBehaviour
{
    [SerializeField] Image healthBar;
    [SerializeField] Text healthText;
    [SerializeField] Image shieldBar;
    [SerializeField] Image shieldBarBackground;
    [SerializeField] Text shieldText;
    [SerializeField] Image shieldTextBackground;

    private SeeSharpController controller;

    void Start()
    {
        controller = FindObjectOfType<SeeSharpController>();
        healthText.text = $"{controller.MaxHealth} / {controller.MaxHealth}";
    }

    void Update()
    {
        UpdateHealth();
        DisplayShield();
    }

    void UpdateHealth()
    {
        int currentHealth = controller.GetCurrentHealth();
        healthText.text = $"{currentHealth} / {controller.MaxHealth}";
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, (float)currentHealth / controller.MaxHealth, 0.1f);
    }

    void DisplayShield()
    {
        int shieldValue = controller.GetCurrentShieldValue();
        int maxShield = controller.GetShieldValue();

        if (shieldValue > 0)
        {
            shieldBarBackground.gameObject.SetActive(true);
            shieldTextBackground.gameObject.SetActive(true);
            shieldBar.fillAmount = (float)shieldValue / maxShield;
            shieldText.text = $"{shieldValue} / {maxShield}";
        }
        else
        {
            shieldBarBackground.gameObject.SetActive(false);
            shieldTextBackground.gameObject.SetActive(false);
        }
    }
}
