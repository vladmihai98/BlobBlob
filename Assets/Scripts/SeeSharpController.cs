using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeeSharpController : Character
{
    private Rigidbody rigidbody;
    private Vector3 velocity;
    private int shieldValue;
    private int currentShieldValue;

    /// <summary>
    /// Keep track of objects that have damaged us so that their colliders can't damage us the next frame.
    /// Remove the objects after a given period of time, say 10s.
    /// </summary>
    private List<GameObject> pastDamagingParticles;

    public int GetCurrentHealth() { return currentHealth; }

    public int GetCurrentShieldValue() { return currentShieldValue; }

    public int GetShieldValue() { return shieldValue; }

    public Vector3 GetVelocity() { return velocity; }

    public void TakeHit(int damageAmount, Resistance resistance)
    {
        currentShieldValue = TakeDamage(damageAmount, resistance, false, currentShieldValue);
        if (currentShieldValue <= 0)
        {
            Ability[] equippedAbilities = GetComponentsInChildren<Ability>();
            foreach (Ability shield in equippedAbilities)
            {
                if (shield.ShieldAmount > 0)
                {
                    Destroy(shield.gameObject);
                }
            }
        }
    }

    public void UseHeal(int healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > MaxHealth)
        {
            currentHealth = MaxHealth;
        }
    }

    public void UseShield(int shieldAmount, float timeOut)
    {
        shieldValue = currentShieldValue = shieldAmount;
        StartCoroutine(DestroyShield(timeOut));
    }

    void Start()
    {
        // Initialise inherited variables.
        currentHealth = MaxHealth;
        currentShieldValue = -1;
        rigidbody = GetComponent<Rigidbody>();
        pastDamagingParticles = new List<GameObject>();
    }

    public override void Interact()
    {
        Move();
    }

    public override void Move()
    {
        ProcessInput();
        rigidbody.MovePosition(transform.position + (velocity * MovementSpeed * Time.deltaTime));
    }

    /// <summary>
    /// Process the keyboard input to decide move direction.
    /// </summary>
    void ProcessInput()
    {
        velocity = Vector3.zero;

        // Move Forward
        if(Input.GetKey(KeyCode.UpArrow))
        {
            velocity.z = 1;
            animator.SetBool("forward", true);
        }
        else if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            velocity.z = 0;
            animator.SetBool("forward", false);
        }

        // Move Right
        if (Input.GetKey(KeyCode.RightArrow))
        {
            velocity.x = 1;
            animator.SetBool("right", true);
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            velocity.x = 0;
            animator.SetBool("right", false);
        }

        // Move Back
        if (Input.GetKey(KeyCode.DownArrow))
        {
            velocity.z = -1;
            animator.SetBool("back", true);
        }
        else if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            velocity.z = 0;
            animator.SetBool("back", false);
        }

        // Move Left
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            velocity.x = -1;
            animator.SetBool("left", true);
        }
        else if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            velocity.x = 0;
            animator.SetBool("left", false);
        }

        if(velocity != Vector3.zero)
        {
            animator.SetBool("move", true);
        }
        else
        {
            animator.SetBool("move", false);
        }
    }

    IEnumerator DestroyShield(float timeOut)
    {
        yield return new WaitForSecondsRealtime(timeOut);
        currentShieldValue = -1;
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleDamage(other.GetComponent<Ability>());
    }

    private void OnParticleCollision(GameObject other)
    {
        if (!pastDamagingParticles.Contains(other))
        {
            pastDamagingParticles.Add(other);
            HandleDamage(other.GetComponent<Ability>());
        }
    }

    private void HandleDamage(Ability ability)
    {
        if(!ability)
        {
            return;
        }

        // Ignore friendly-fire, e.g. abilities that should damage enemies and not our friends.
        if(ability.TypeOfTarget == Ability.TargetType.Enemy)
        {
            return;
        }

        if (ability.AttackDamage > 0)
        {
            currentShieldValue = TakeDamage(ability.AttackDamage, Resistance.UseArmor, false, currentShieldValue);
        }
        else if (ability.AbilityPower > 0)
        {
            currentShieldValue = TakeDamage(ability.AbilityPower, Resistance.UseMagicResist, false, currentShieldValue);
        }
        else
        {
            print($"[WARNING] No damage on {transform.name} from {ability.name}");
        }

        if (currentShieldValue <= 0)
        {
            Ability[] equippedAbilities = GetComponentsInChildren<Ability>();
            foreach (Ability shield in equippedAbilities)
            {
                if (shield.ShieldAmount > 0)
                {
                    Destroy(shield.gameObject);
                }
            }
        }
    }
}
