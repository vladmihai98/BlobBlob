using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MontyHUDController : MonoBehaviour
{
    [Header("References to managers.")]
    [SerializeField] MontyController controller;
    [SerializeField] SpellHandler handler;
    
    [Header("References to HUD items.")]
    [SerializeField] Image healthBar;
    [SerializeField] Text healthText;
    [SerializeField] Image manaBar;
    [SerializeField] Text manaText;
    [SerializeField] Image basicHealCD;
    [SerializeField] Text basicHealCDText;
    [SerializeField] Text basicHealCost;
    [SerializeField] Image techShieldCD;
    [SerializeField] Text techShieldCDText;
    [SerializeField] Text techShieldCost;
    [SerializeField] Image energySlashCD;
    [SerializeField] Text energySlashCDText;
    [SerializeField] Text energySlashCost;

    void Start()
    {
        healthText.text = $"{controller.MaxHealth} / {controller.MaxHealth}";
        manaText.text = $"{controller.MaxMana} / {controller.MaxMana}";

        basicHealCost.text = "" + handler.GetBasicHealCost();
        techShieldCost.text = "" + handler.GetTechShieldCost();
        energySlashCost.text = "" + handler.GetEnergySlashCost();

        basicHealCD.fillAmount = 0f;
        basicHealCDText.text = string.Empty;
    }

    void Update()
    {
        UpdateHealth();
        UpdateMana();
    }

    void UpdateHealth()
    {
        int currentHealth = controller.GetCurrentHealth();
        healthText.text = $"{currentHealth} / {controller.MaxHealth}";
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, (float) currentHealth / controller.MaxHealth, 0.1f);
    }

    void UpdateMana()
    {
        int currentMana = controller.GetCurrentMana();
        manaText.text = $"{currentMana} / {controller.MaxMana}";
        manaBar.fillAmount = Mathf.Lerp(manaBar.fillAmount, (float) currentMana / controller.MaxMana, 0.1f);
    }

    public void PutBasicHealOnCooldown(int seconds)
    {
        StartCoroutine(UpdateBHCD(seconds));
    }

    IEnumerator UpdateBHCD(int cooldown)
    {
        int currCd = cooldown;
        basicHealCD.fillAmount = 1f;

        while(currCd > 0)
        {
            print($"cd {cooldown} and currcd {currCd}");
            currCd--;
            basicHealCDText.text = $"{currCd} s";
            basicHealCD.fillAmount -= (float) 1 / cooldown;
            yield return new WaitForSecondsRealtime(1f);
        }

        basicHealCD.fillAmount = 0f;
        basicHealCDText.text = string.Empty;
    }
}