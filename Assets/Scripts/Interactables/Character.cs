using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Character : Interactable
{
    public Image healthBar;
    public Animator animator;

    [Header("Generic Stats")]
    public int MaxHealth;
    public int Mana;
    public int MovementSpeed;
    public int AttackRange;
    public bool ShowRange;

    [Header("Offensive Stats")]
    public int AttackDamage;
    public int AbilityPower;
    public float AttackSpeed;
    public float CastingSpeed;

    [Header("Defensive Stats")]
    public int Armor;
    public int MagicResist;

    protected int currentHealth;
    protected Transform initialPosition;
    protected bool isAggroed = false;

    /// <summary>
    /// To identify what to use to reduce damage.
    /// </summary>
    public enum Resistance { UseArmor, UseMagicResist };

    /// <summary>
    /// Handles character movement.
    /// Requires implementation.
    /// </summary>
    public virtual void Move()
    {
        // no implementation;
    }

    /// <summary>
    /// What goes in the Update Unity callback.
    /// Requires implementation.
    /// </summary>
    public virtual void Interact()
    {
        // no implementation;
    }

    /// <summary>
    /// Handle action such as shooting bullets/casting spells.
    /// Requires implementation.
    /// </summary>
    public virtual void PerformAction()
    {
        // no implementation;
    }

    /// <summary>
    /// Handle taking damage.
    /// </summary>
    protected void TakeDamage(int damage, Resistance resistance)
    {
        // If it is physical damage we want to use armor to reduce it.
        if (resistance == Resistance.UseArmor)
        {
            // Reduce damage effectiveness based on armor value.
            damage -= Armor;
            damage = damage < 0 ? 0 : damage;

            // Update current health and the health HUD.
            currentHealth -= damage;
            healthBar.fillAmount = (float) currentHealth / MaxHealth;

            if (currentHealth <= 0)
            {
                Die();
            }
        }
        // It is magic damage so use magic resist.
        else
        {
            // Reduce damage effectiveness based on magic resist value.
            damage -= MagicResist;
            damage = damage < 0 ? 0 : damage;

            // Update current health and the health HUD.
            currentHealth -= damage;
            healthBar.fillAmount = (float) currentHealth / MaxHealth;

            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    /// <summary>
    /// Handle death.
    /// </summary>
    private void Die()
    {
        print($"[INFO] Character {transform.name} has died.");
        animator.SetTrigger("die");
        StartCoroutine(DestroyGameObject());
    }

    IEnumerator DestroyGameObject()
    {
        yield return new WaitForSecondsRealtime(10f);
        Destroy(gameObject);
    }

    void Update()
    {
        Interact();
    }
}
