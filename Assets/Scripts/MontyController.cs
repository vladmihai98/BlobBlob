using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MontyController : Character
{
    [Header("Extra Stats")]
    [SerializeField] float jumpHeight = 300;
    [SerializeField] GameObject BasicHeal;
    [SerializeField] GameObject TechShield;
    [SerializeField] GameObject EnergySlash;
    [SerializeField] GameObject AoeHeal;

    private Rigidbody rigidbody;
    private Vector3 velocity;

    /// <summary>
    /// Keep track of objects that have damaged us so that their colliders can't damage us the next frame.
    /// Remove the objects after a given period of time, say 10s.
    /// </summary>
    private List<GameObject> pastDamagingParticles;
    private bool isGrounded;
    private GameObject seeSharp;
    private Transform spellTarget;

    /// <summary>
    /// Key: The name of the spell to be cast. 
    /// Value: List with 3 elements, 0: Whether we can cast spell, 1: ManaCost, 2: SpellCooldown.
    /// </summary>
    private Dictionary<string, int[]> spellsStats;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        currentHealth = MaxHealth;
        currentMana = MaxMana;
        isGrounded = true;

        pastDamagingParticles = new List<GameObject>();
        seeSharp = GameObject.Find("SeeSharp");

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

        // Cast BasicHeal.
        if(Input.GetKeyDown(KeyCode.Z))
        {
            // If we can cast the spell (i.e. it's not on cooldown) and we have the mana for it
            if(spellsStats[BasicHeal.name][0] == 1 && spellsStats[BasicHeal.name][1] <= currentMana)
            {
                spellTarget = transform;
                InstantiateSpellAtTarget(BasicHeal, false, true);

                if(spellTarget.Equals(transform))
                {
                    UseHeal(BasicHeal.GetComponent<Ability>().HealAmount);
                }
                else
                {
                    var ctrl = spellTarget.GetComponent<SeeSharpController>();
                    ctrl.UseHeal(BasicHeal.GetComponent<Ability>().HealAmount);
                }

                currentMana -= spellsStats[BasicHeal.name][1];
                manaBar.fillAmount = (float) currentMana/ MaxMana;
                spellsStats[BasicHeal.name][0] = 0;
                StartCoroutine(PutSpellOnCooldown(BasicHeal.name, spellsStats[BasicHeal.name][2]));
            }
        }

        // Cast TechShield.
        if (Input.GetKeyDown(KeyCode.X))
        {
            if(spellsStats[TechShield.name][0] == 1 && spellsStats[TechShield.name][1] <= currentMana)
            {
                spellTarget = transform;
                InstantiateSpellAtTarget(TechShield, true, true);

                currentMana -= spellsStats[TechShield.name][1];
                manaBar.fillAmount = (float)currentMana / MaxMana;
                spellsStats[TechShield.name][0] = 0;
                StartCoroutine(PutSpellOnCooldown(TechShield.name, spellsStats[TechShield.name][2]));
            }
        }

        // Cast EnergySlash.
        if(Input.GetKeyDown(KeyCode.C))
        {
            if(spellsStats[EnergySlash.name][0] == 1 && spellsStats[EnergySlash.name][1] <= currentMana)
            {
                spellTarget = transform;
                InstantiateSpellAtTarget(EnergySlash, true);

                currentMana -= spellsStats[EnergySlash.name][1];
                manaBar.fillAmount = (float)currentMana / MaxMana;
                spellsStats[EnergySlash.name][0] = 0;
                StartCoroutine(PutSpellOnCooldown(EnergySlash.name, spellsStats[EnergySlash.name][2]));
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
        yield return new WaitForSeconds(0.5f);

        rigidbody.AddForce(0, jumpHeight * Time.deltaTime, 0);
    }

    void InstantiateSpellAtTarget(GameObject spell, bool adjustHeight = false, bool isObjectNested = false)
    {
        Vector3 targetPosition = spellTarget.position;
        if(adjustHeight)
        {
            targetPosition.y += 5f;
        }

        GameObject instance = Instantiate(spell, targetPosition, Quaternion.identity);
        if(isObjectNested)
        {
            instance.transform.parent = spellTarget;
        }
    }

    IEnumerator PutSpellOnCooldown(string spellName, int cooldown)
    {
        yield return new WaitForSecondsRealtime(cooldown);
        spellsStats[spellName][0] = 1;
    }

    void UseHeal(int healAmount)
    {
        currentHealth += healAmount;
        if(currentHealth > MaxHealth)
        {
            currentHealth = MaxHealth;
        }
        healthBar.fillAmount = (float) currentHealth / MaxHealth;
    }

    public void TakeHit(int damageAmount, Resistance resistance)
    {
        TakeDamage(damageAmount, resistance);
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
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

        HandleDamage(other.GetComponent<Ability>());
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
            TakeDamage(ability.AttackDamage, Resistance.UseArmor);
        }
        else if (ability.AbilityPower > 0)
        {
            TakeDamage(ability.AbilityPower, Resistance.UseMagicResist);
        }
        else
        {
            print($"[WARNING] No damage on {transform.name} from {ability.name}");
        }
    }
}
