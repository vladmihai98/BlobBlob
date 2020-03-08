using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] Image shieldBar;
    [SerializeField] Image shieldBarBackground;
    [SerializeField] Text shieldText;
    [SerializeField] Image shieldTextBackground;
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

    /// <summary>
    /// Dictionary holding the references to the CD image and text of each spell.
    /// Key: Spell name, Value: array with 2 objects: CD image and CD text.
    /// </summary>
    private Dictionary<string, Tuple<Image, Text>> spellsCooldownUpdateable;

    void Start()
    {
        healthText.text = $"{controller.MaxHealth} / {controller.MaxHealth}";
        manaText.text = $"{controller.MaxMana} / {controller.MaxMana}";

        basicHealCost.text = "" + handler.GetBasicHealCost();
        techShieldCost.text = "" + handler.GetTechShieldCost();
        energySlashCost.text = "" + handler.GetEnergySlashCost();

        basicHealCD.fillAmount = 0f;
        basicHealCDText.text = string.Empty;
        techShieldCD.fillAmount = 0f;
        techShieldCDText.text = string.Empty;
        energySlashCD.fillAmount = 0f;
        energySlashCDText.text = string.Empty;

        spellsCooldownUpdateable = new Dictionary<string, Tuple<Image, Text>>();
        spellsCooldownUpdateable.Add("BasicHeal", new Tuple<Image, Text>(basicHealCD, basicHealCDText));
        spellsCooldownUpdateable.Add("TechShield", new Tuple<Image, Text>(techShieldCD, techShieldCDText));
        spellsCooldownUpdateable.Add("EnergySlash", new Tuple<Image, Text>(energySlashCD, energySlashCDText));
    }

    void Update()
    {
        UpdateHealth();
        DisplayShield();
        UpdateMana();
    }

    void UpdateHealth()
    {
        int currentHealth = controller.GetCurrentHealth();
        healthText.text = $"{currentHealth} / {controller.MaxHealth}";
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, (float) currentHealth / controller.MaxHealth, 0.1f);
    }

    void DisplayShield()
    {
        int shieldValue = controller.GetCurrentShieldValue();
        int maxShield = controller.GetShieldValue();

        if(shieldValue > 0)
        {
            shieldBarBackground.gameObject.SetActive(true);
            shieldTextBackground.gameObject.SetActive(true);
            shieldBar.fillAmount = (float) shieldValue / maxShield;
            shieldText.text = $"{shieldValue} / {maxShield}";
        }
        else
        {
            shieldBarBackground.gameObject.SetActive(false);
            shieldTextBackground.gameObject.SetActive(false);
        }
    }

    void UpdateMana()
    {
        int currentMana = controller.GetCurrentMana();
        manaText.text = $"{currentMana} / {controller.MaxMana}";
        manaBar.fillAmount = Mathf.Lerp(manaBar.fillAmount, (float) currentMana / controller.MaxMana, 0.1f);
    }

    public void PutBasicHealOnCooldown(int seconds)
    {
        StartCoroutine(UpdateSpellCooldown("BasicHeal", seconds));
    }

    public void PutTechShieldOnCooldown(int seconds)
    {
        StartCoroutine(UpdateSpellCooldown("TechShield", seconds));
    }

    public void PutEnergySlashOnCooldown(int seconds)
    {
        StartCoroutine(UpdateSpellCooldown("EnergySlash", seconds));
    }

    IEnumerator UpdateSpellCooldown(string spellName, int cooldown)
    {
        int currentCd = cooldown;
        Image image = spellsCooldownUpdateable[spellName].Item1;
        Text text = spellsCooldownUpdateable[spellName].Item2;

        image.fillAmount = 1f;
        text.text = $"{currentCd} s";
        yield return new WaitForSecondsRealtime(1f);

        while (currentCd > 0)
        {
            image.fillAmount -= (float) 1 / cooldown;
            currentCd--;
            if(currentCd == 0)
            {
                text.text = string.Empty;
            }
            else
            {
                text.text = $"{currentCd} s";
            }
            yield return new WaitForSecondsRealtime(1f);
        }
    }
}