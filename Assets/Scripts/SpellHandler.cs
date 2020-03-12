using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellHandler : MonoBehaviour
{
    [SerializeField] MontyHUDController hudController;

    [Header("Spells")]
    [SerializeField] GameObject BasicHeal;
    [SerializeField] GameObject TechShield;
    [SerializeField] GameObject EnergySlash;
    [SerializeField] GameObject AoeHeal;

    /// <summary>
    /// Key: The name of the spell to be cast. 
    /// Value: List with 3 elements, 0: Whether we can cast spell, 1: ManaCost, 2: SpellCooldown.
    /// </summary>
    private Dictionary<string, int[]> spellsStats;

    private MontyController controller;
    private Transform spellTarget;
    private int currentMana;

    void Awake()
    {
        controller = GetComponent<MontyController>();
        currentMana = controller.MaxMana;

        int basicHealManaCost = BasicHeal.GetComponent<Ability>().ManaCost;
        int basicHealCooldown = BasicHeal.GetComponent<Ability>().Cooldown;
        int techShieldManaCost = TechShield.GetComponent<Ability>().ManaCost;
        int techShieldCooldown = TechShield.GetComponent<Ability>().Cooldown;
        int energySlashManaCost = EnergySlash.GetComponent<Ability>().ManaCost;
        int energySlashCooldown = EnergySlash.GetComponent<Ability>().Cooldown;
        int aoeHealManaCost = AoeHeal.GetComponent<Ability>().ManaCost;
        int aoeHealCooldown = AoeHeal.GetComponent<Ability>().Cooldown;

        spellsStats = new Dictionary<string, int[]>();
        spellsStats.Add(BasicHeal.name, new int[3] { 1, basicHealManaCost, basicHealCooldown });
        spellsStats.Add(TechShield.name, new int[3] { 1, techShieldManaCost, techShieldCooldown });
        spellsStats.Add(EnergySlash.name, new int[3] { 1, energySlashManaCost, energySlashCooldown });
        spellsStats.Add(AoeHeal.name, new int[3] { 1, aoeHealManaCost, aoeHealCooldown });
    }

    void Update()
    {
        currentMana = controller.GetCurrentMana();    
    }

    /// <summary>
    /// The spell Monty casts when pressing Z.
    /// </summary>
    /// <returns>The mana pool available after casting the spell.</returns>
    public int CastBasicHeal(Transform newTarget = null)
    {
        // If we can cast the spell (i.e. it's not on cooldown) and we have the mana for it
        if (spellsStats[BasicHeal.name][0] == 1 && spellsStats[BasicHeal.name][1] <= currentMana)
        {
            spellTarget = transform;
            if(newTarget)
            {
                spellTarget = newTarget;
            }

            InstantiateSpellAtTarget(BasicHeal, false, true);

            if (spellTarget.Equals(transform))
            {
                controller.UseHeal(BasicHeal.GetComponent<Ability>().HealAmount);
            }
            else
            {
                SeeSharpController ctrl = spellTarget.GetComponent<SeeSharpController>();
                ctrl.UseHeal(BasicHeal.GetComponent<Ability>().HealAmount);
            }

            currentMana -= spellsStats[BasicHeal.name][1];
            currentMana = currentMana < 0 ? 0 : currentMana;

            // Now that we've cast the spell, make it unavailable for its cooldown time.
            spellsStats[BasicHeal.name][0] = 0;
            StartCoroutine(PutSpellOnCooldown(BasicHeal.name, spellsStats[BasicHeal.name][2]));
            hudController.PutBasicHealOnCooldown(spellsStats[BasicHeal.name][2]);

            return currentMana;
        }

        return -1;
    }

    /// <summary>
    /// The spell Monty casts when pressing X.
    /// </summary>
    /// <returns>The mana pool available after casting the spell.</returns>
    public int CastTechShield(Transform newTarget = null)
    {
        if (spellsStats[TechShield.name][0] == 1 && spellsStats[TechShield.name][1] <= currentMana)
        {
            spellTarget = transform;
            if (newTarget)
            {
                spellTarget = newTarget;
            }

            InstantiateSpellAtTarget(TechShield, true, true);

            if(spellTarget.Equals(transform))
            {
                controller.UseShield(TechShield.GetComponent<Ability>().ShieldAmount, TechShield.GetComponent<Ability>().Duration);
            }
            else
            {
                SeeSharpController seeSharpController = spellTarget.GetComponent<SeeSharpController>();
                seeSharpController.UseShield(TechShield.GetComponent<Ability>().ShieldAmount, TechShield.GetComponent<Ability>().Duration);
            }

            currentMana -= spellsStats[TechShield.name][1];
            currentMana = currentMana < 0 ? 0 : currentMana;

            spellsStats[TechShield.name][0] = 0;
            StartCoroutine(PutSpellOnCooldown(TechShield.name, spellsStats[TechShield.name][2]));
            hudController.PutTechShieldOnCooldown(spellsStats[TechShield.name][2]);

            return currentMana;
        }

        return -1;
    }

    /// <summary>
    /// The spell Monty casts when pressing C.
    /// </summary>
    /// <returns>The mana pool available after casting the spell.</returns>
    public int CastEnergySlash(Transform newTarget = null)
    {
        if (spellsStats[EnergySlash.name][0] == 1 && spellsStats[EnergySlash.name][1] <= currentMana)
        {
            spellTarget = transform;
            if (newTarget)
            {
                spellTarget = newTarget;
            }

            InstantiateSpellAtTarget(EnergySlash, true);

            currentMana -= spellsStats[EnergySlash.name][1];
            currentMana = currentMana < 0 ? 0 : currentMana;

            spellsStats[EnergySlash.name][0] = 0;
            StartCoroutine(PutSpellOnCooldown(EnergySlash.name, spellsStats[EnergySlash.name][2]));
            hudController.PutEnergySlashOnCooldown(spellsStats[EnergySlash.name][2]);

            return currentMana;
        }

        return -1;
    }

    /// <summary>
    /// Get the mana cost of the BasicHeal spell.
    /// </summary>
    /// <returns>An int representing the mana cost of the BasicHeal spell.</returns>
    public int GetBasicHealCost()
    {
        return spellsStats[BasicHeal.name][1];
    }

    /// <summary>
    /// Get the mana cost of the TechShield spell.
    /// </summary>
    /// <returns>An int representing the mana cost of the TechShield spell.</returns>
    public int GetTechShieldCost()
    {
        return spellsStats[TechShield.name][1];
    }

    /// <summary>
    /// Get the mana cost of the EnergySlash spell.
    /// </summary>
    /// <returns>An int representing the mana cost of the EnergySlash spell.</returns>
    public int GetEnergySlashCost()
    {
        return spellsStats[EnergySlash.name][1];
    }

    void InstantiateSpellAtTarget(GameObject spell, bool adjustHeight = false, bool isObjectNested = false)
    {
        Vector3 targetPosition = spellTarget.position;
        if (adjustHeight)
        {
            targetPosition.y += 5f;
        }

        GameObject instance = Instantiate(spell, targetPosition, Quaternion.identity);
        if (isObjectNested)
        {
            instance.transform.parent = spellTarget;
        }
    }

    IEnumerator PutSpellOnCooldown(string spellName, int cooldown)
    {
        yield return new WaitForSecondsRealtime(cooldown);
        spellsStats[spellName][0] = 1;
    }
}
