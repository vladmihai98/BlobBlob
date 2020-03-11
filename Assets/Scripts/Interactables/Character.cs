using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Character : Interactable
{
    public Image healthBar;
    public Image manaBar;
    public Animator animator;
    public ParticleSystem ManaParticles;
    [Tooltip("How many mana points to be spawned on the ground.")]
    public int ManaPoints;

    [Header("Generic Stats")]
    public int MaxHealth;
    public int MaxMana;
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
    protected int currentMana;
    protected bool isAlive = true;
    protected Transform initialPosition;
    protected bool isAggroed = false;
    protected bool isEnraged = false;

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
    protected int TakeDamage(int damage, Resistance resistance, bool updateHUD = true, int shieldValue = -1)
    {
        // If it is physical damage we want to use armor to reduce it.
        if (resistance == Resistance.UseArmor)
        {
            // Reduce damage effectiveness based on armor value.
            damage -= Armor;

            // Reduce shield value if active
            if(shieldValue > -1)
            {
                if(shieldValue > damage)
                {
                    shieldValue -= damage;
                    damage = 0;
                }
                else if(shieldValue < damage)
                {
                    damage -= shieldValue;
                    shieldValue = -1;
                }
                else
                {
                    shieldValue = -1;
                    damage = 0;
                }
            }

            // Reset damage if it fell below zero.
            damage = damage < 0 ? 0 : damage;

            // Update current health and the health HUD.
            currentHealth -= damage;
            currentHealth = currentHealth < 0 ? 0 : currentHealth;
            if (updateHUD)
            {
                healthBar.fillAmount = (float) currentHealth / MaxHealth;
            }

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

            // Reduce shield value if active
            if (shieldValue > -1)
            {
                if (shieldValue > damage)
                {
                    shieldValue -= damage;
                    damage = 0;
                }
                else if (shieldValue < damage)
                {
                    damage -= shieldValue;
                    shieldValue = -1;
                }
                else
                {
                    shieldValue = -1;
                    damage = 0;
                }
            }

            // Reset damage if it fell below zero.
            damage = damage < 0 ? 0 : damage;

            // Update current health and the health HUD.
            currentHealth -= damage;
            currentHealth = currentHealth < 0 ? 0 : currentHealth;
            if(updateHUD)
            {
                healthBar.fillAmount = (float) currentHealth / MaxHealth;
            }

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        return shieldValue;
    }

    /// <summary>
    /// Handle death.
    /// </summary>
    private void Die()
    {
        if(isAlive)
        {
            print($"[INFO] Character {transform.name} has died.");

            // Put the object in final state.
            isAlive = false;
            var navMeshAgent = transform.GetComponent<NavMeshAgent>();
            if(navMeshAgent)
            {
                navMeshAgent.enabled = false;
            }
            var rigidBody = transform.GetComponent<Rigidbody>();
            if(rigidBody)
            {
                rigidBody.isKinematic = true;
            }
            transform.GetComponent<Collider>().enabled = false;
            animator.SetTrigger("die");

            if(ManaParticles)
            {
                var emiss = ManaParticles.emission;
                emiss.burstCount = ManaPoints;
                Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y + 5f, transform.position.z);
                Instantiate(ManaParticles, spawnPosition, Quaternion.identity);
            }

            StartCoroutine(DestroyGameObject());
        }
    }

    IEnumerator DestroyGameObject()
    {
        yield return new WaitForSecondsRealtime(10f);
        Destroy(gameObject);
    }

    void Update()
    {
        if(isAlive)
        {
            Interact();
        }
    }
}
