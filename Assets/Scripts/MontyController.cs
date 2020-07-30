using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MontyController : Character
{
    [Header("Extra Stats")]
    [SerializeField] float jumpHeight = 300;

    private SpellHandler spellHandler;
    private Rigidbody rigidbody;
    private Vector3 velocity;
    private bool isGrounded;
    private int shieldValue;
    private int currentShieldValue;
    private GameController gameController;
    private SeeSharpController seeSharpController;

    /// <summary>
    /// Keep track of objects that have damaged us so that their colliders can't damage us the next frame.
    /// Remove the objects after a given period of time, say 10s.
    /// </summary>
    private List<GameObject> pastDamagingParticles;

    public int GetCurrentHealth() { return currentHealth; }

    public int GetCurrentMana() { return currentMana; }

    public int GetCurrentShieldValue() { return currentShieldValue; }

    public int GetMovementSpeed() { return MovementSpeed; }

    public int GetShieldValue() { return shieldValue; }

    public void SetMovementSpeed(int newSpeed) { MovementSpeed = newSpeed; }

    public void TakeHit(int damageAmount, Resistance resistance)
    {
        currentShieldValue = TakeDamage(damageAmount, resistance, false, currentShieldValue);
        if(currentShieldValue <= 0)
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
        currentMana = MaxMana;

        isGrounded = true;
        currentShieldValue = -1;
        gameController = FindObjectOfType<GameController>();
        pastDamagingParticles = new List<GameObject>();
        rigidbody = GetComponent<Rigidbody>();
        seeSharpController = FindObjectOfType<SeeSharpController>();
        spellHandler = GetComponent<SpellHandler>();
    }

    public override void Interact()
    {
        if(gameController.IsMontyAlive())
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
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Z))
        //if (Input.GetKeyDown(KeyCode.Q))
        {
            float distanceFromSeeSharp = float.MaxValue;
            if(gameController.IsSeeSharpAlive())
            {
                distanceFromSeeSharp = Vector3.Distance(seeSharpController.transform.position, transform.position);
            }

            if(gameController.IsSeeSharpAlive() && distanceFromSeeSharp < AttackRange)
            {
                int result = spellHandler.CastBasicHeal(seeSharpController.transform);
                if(result >= 0)
                {
                    currentMana = result;
                }
            }
        }

        // Cast TechShield on SeeSharp.
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.X))
        //if (Input.GetKeyDown(KeyCode.E))
        {
            float distanceFromSeeSharp = float.MaxValue;
            if (gameController.IsSeeSharpAlive())
            {
                distanceFromSeeSharp = Vector3.Distance(seeSharpController.transform.position, transform.position);
            }

            if (distanceFromSeeSharp < AttackRange)
            {
                int result = spellHandler.CastTechShield(seeSharpController.transform);
                if (result >= 0)
                {
                    currentMana = result;
                }
            }
        }

        // Cast EnergySlash from SeeSharp's position.
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.C))
        //if (Input.GetKeyDown(KeyCode.R))
        {
            float distanceFromSeeSharp = float.MaxValue;
            if (gameController.IsSeeSharpAlive())
            {
                distanceFromSeeSharp = Vector3.Distance(seeSharpController.transform.position, transform.position);
            }

            if (distanceFromSeeSharp < AttackRange)
            {
                int result = spellHandler.CastEnergySlash(seeSharpController.transform);
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
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            isGrounded = false;

            // Start the animation.
            animator.SetTrigger("jump");

            // Perform jump after delay, since character ducks before jumping.
            StartCoroutine(PerformJump());
        }

        // Kudos to this video https://www.youtube.com/watch?v=7KiK0Aqtmzc
        float fallMultiplier = 2.5f;
        float jumpMultiplier = 2f;
        if(rigidbody.velocity.y < 0)
        {
            rigidbody.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if(rigidbody.velocity.y > 0)
        {
            rigidbody.velocity += Vector3.up * Physics.gravity.y * (jumpMultiplier - 1) * Time.deltaTime;
        }
    }

    IEnumerator PerformJump()
    {
        yield return new WaitForSeconds(0.57f);
        rigidbody.velocity = Vector3.up * jumpHeight;
    }

    IEnumerator DestroyShield(float timeOut)
    {
        yield return new WaitForSecondsRealtime(timeOut);
        currentShieldValue = -1;
    }

    private void OnParticleCollision(GameObject other)
    {
        if (!pastDamagingParticles.Contains(other))
        {
            pastDamagingParticles.Add(other);
            HandleDamage(other.GetComponent<Ability>());
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Register hitting ground.
        isGrounded = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("ManaParticles"))
        {
            GainMana(other.GetComponent<ParticleSystem>().emission.burstCount);
            Destroy(other.gameObject);
        }

        Ability ability = other.GetComponent<Ability>();
        if(ability)
        {
            HandleDamage(ability);
        }
    }

    public void GainMana(int manaPoints)
    {
        currentMana += manaPoints * 20;
        if(currentMana > MaxMana)
        {
            currentMana = MaxMana;
        }
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

        if(currentShieldValue <= 0)
        {
            Ability[] equippedAbilities = GetComponentsInChildren<Ability>();
            foreach(Ability shield in equippedAbilities)
            {
                if(shield.ShieldAmount > 0)
                {
                    Destroy(shield.gameObject);
                }
            }
        }
    }
}
