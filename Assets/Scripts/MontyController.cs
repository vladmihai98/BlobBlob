using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MontyController : Character
{
    [Header("Extra Stats")]
    [SerializeField] float jumpHeight = 300;
    [SerializeField] Transform seeSharp;

    private SpellHandler spellHandler;
    private Rigidbody rigidbody;
    private Vector3 velocity;

    /// <summary>
    /// Keep track of objects that have damaged us so that their colliders can't damage us the next frame.
    /// Remove the objects after a given period of time, say 10s.
    /// </summary>
    private List<GameObject> pastDamagingParticles;
    private bool isGrounded;

    void Start()
    {
        spellHandler = GetComponent<SpellHandler>();
        rigidbody = GetComponent<Rigidbody>();
        currentHealth = MaxHealth;
        currentMana = MaxMana;
        isGrounded = true;

        pastDamagingParticles = new List<GameObject>();
    }

    public override void Interact()
    {
        if(gameObject)
        {
            ProcessInput();
            Move();
            Jump();
        }
    }

    /// <summary>
    /// Process the keyboard input to decide move direction.
    /// </summary>
    void ProcessInput()
    {
        velocity = Vector3.zero;

        // Move Forward
        if (Input.GetKey(KeyCode.W))
        {
            velocity.z = 1;
            animator.SetBool("forward", true);
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            velocity.z = 0;
            animator.SetBool("forward", false);
        }

        // Move Right
        if (Input.GetKey(KeyCode.D))
        {
            velocity.x = 1;
            animator.SetBool("right", true);
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            velocity.x = 0;
            animator.SetBool("right", false);
        }

        // Move Back
        if (Input.GetKey(KeyCode.S))
        {
            velocity.z = -1;
            animator.SetBool("back", true);
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            velocity.z = 0;
            animator.SetBool("back", false);
        }

        // Move Left
        if (Input.GetKey(KeyCode.A))
        {
            velocity.x = -1;
            animator.SetBool("left", true);
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            velocity.x = 0;
            animator.SetBool("left", false);
        }

        if (velocity != Vector3.zero)
        {
            animator.SetBool("move", true);
        }
        else
        {
            animator.SetBool("move", false);
        }

        // Cast BasicHeal on SeeSharp.
        //if (Input.GetKey(KeyCode.RightControl) && Input.GetKeyDown(KeyCode.Z))
        if (Input.GetKeyDown(KeyCode.Q))
        {
            float distanceFromSeeSharp = Vector3.Distance(seeSharp.position, transform.position);

            if(distanceFromSeeSharp < AttackRange)
            {
                int result = spellHandler.CastBasicHeal(seeSharp);
                if(result >= 0)
                {
                    currentMana = result;
                }
            }
        }

        // Cast TechShield on SeeSharp.
        //if (Input.GetKey(KeyCode.RightControl) && Input.GetKeyDown(KeyCode.X))
        if (Input.GetKeyDown(KeyCode.E))
        {
            float distanceFromSeeSharp = Vector3.Distance(seeSharp.position, transform.position);

            if (distanceFromSeeSharp < AttackRange)
            {
                int result = spellHandler.CastTechShield(seeSharp);
                if (result >= 0)
                {
                    currentMana = result;
                }
            }
        }

        // Cast EnergySlash from SeeSharp's position.
        //if (Input.GetKey(KeyCode.RightControl) && Input.GetKeyDown(KeyCode.C))
        if (Input.GetKeyDown(KeyCode.R))
        {
            float distanceFromSeeSharp = Vector3.Distance(seeSharp.position, transform.position);

            if(distanceFromSeeSharp < AttackRange)
            {
                int result = spellHandler.CastEnergySlash(seeSharp);
                if (result >= 0)
                {
                    currentMana = result;
                }
            }
        }

        // Cast BasicHeal on Monty.
        if (Input.GetKeyDown(KeyCode.Z))
        {
            int result = spellHandler.CastBasicHeal();
            if(result >= 0)
            {
                currentMana = result;
            }
        }

        // Cast TechShield on Monty.
        if (Input.GetKeyDown(KeyCode.X))
        {
            int result = spellHandler.CastTechShield();
            if (result >= 0)
            {
                currentMana = result;
            }
        }

        // Cast EnergySlash on Monty.
        if(Input.GetKeyDown(KeyCode.C))
        {
            int result = spellHandler.CastEnergySlash();
            if (result >= 0)
            {
                currentMana = result;
            }
        }
    }

    void Move()
    {
        rigidbody.MovePosition(transform.position + (velocity * MovementSpeed * Time.deltaTime));
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            isGrounded = false;

            // Start the animation.
            animator.SetTrigger("jump");

            // Perform jump after delay, since character ducks before jumping.
            StartCoroutine(PerformJump());
        }
    }

    IEnumerator PerformJump()
    {
        yield return new WaitForSeconds(1.15f);

        rigidbody.AddForce(0, jumpHeight, 0);
    }

    public void UseHeal(int healAmount)
    {
        currentHealth += healAmount;
        if(currentHealth > MaxHealth)
        {
            currentHealth = MaxHealth;
        }
    }

    public void TakeHit(int damageAmount, Resistance resistance)
    {
        TakeDamage(damageAmount, resistance, false);
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetCurrentMana()
    {
        return currentMana;
    }

    public int GetMovementSpeed()
    {
        return MovementSpeed;
    }

    public void SetMovementSpeed(int newSpeed)
    {
        MovementSpeed = newSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        print("3");

        // Register hitting ground.
        isGrounded = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        print("2");

        Ability ability = other.GetComponent<Ability>();
        if(ability)
        {
            HandleDamage(ability);
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        print($"1");

        //if (!pastDamagingParticles.Contains(other))
        //{
        //    pastDamagingParticles.Add(other);
        //    HandleDamage(other.GetComponent<Ability>());
        //}
    }

    void OnParticleTrigger()
    {
        print("4");

        List<ParticleSystem.Particle> enter = new List<ParticleSystem.Particle>();
        var wow = ManaParticles.GetComponent<ParticleSystem>();
        int idc = wow.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);

        print($"atatea boss {enter.Count}");

        enter.ForEach(x =>
        {
            print($"do we even have a name {x.position}");
        });
    }

    private void HandleDamage(Ability ability)
    {
        // Ignore friendly-fire, e.g. abilities that should damage enemies and not our friends.
        if (ability.TypeOfTarget == Ability.TargetType.Enemy)
        {
            return;
        }

        if (ability.AttackDamage > 0)
        {
            TakeDamage(ability.AttackDamage, Resistance.UseArmor, false);
        }
        else if (ability.AbilityPower > 0)
        {
            TakeDamage(ability.AbilityPower, Resistance.UseMagicResist, false);
        }
        else
        {
            print($"[WARNING] No damage on {transform.name} from {ability.name}");
        }
    }
}
