using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ConjurerController : Character
{
    [Header("Extra Stats")]
    [SerializeField] Transform seeSharp;
    [SerializeField] Transform monty;
    [SerializeField] GameObject spell;
    [SerializeField] Material material;

    private NavMeshAgent agent; 
    private Color color;
    private bool canCastSpell = true;

    void Start()
    {
        // Initialise inherited variables.
        initialPosition = transform;
        currentHealth = MaxHealth;

        color = material.color;
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = AttackRange;
    }

    void Update()
    {
        if (isAggroed && didPlayerIgnoreMe())
        {
            BecomeEnraged();
        }

        Transform playerToAttack = isPlayerInAttackRange();
        if (playerToAttack)
        {
            AttackPlayer(playerToAttack);
        }
        else
        {
            if (isAggroed)
            {
                ChaseClosestPlayer();
            }
        }
    }

    /// <summary>
    /// For convenience the enemies are placed facing Z-.
    /// So the players ignore the enemies when they have gone past the initial position plus range in the Z+ direction.
    /// </summary>
    /// <returns>True if player ignored enemy, false otherwise.</returns>
    bool didPlayerIgnoreMe()
    {
        if(initialPosition.position.z < monty.position.z || 
           initialPosition.position.z < seeSharp.position.z)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Buff base stats so that the character becomes more threatening for being ignored.
    /// </summary>
    void BecomeEnraged()
    {

    }

    /// <summary>
    /// Retrieve the position of the nearest Player in range.
    /// </summary>
    /// <returns>The transform of the nearest enemy in range or null.</returns>
    Transform isPlayerInAttackRange()
    {
        Transform playerToAttack = null;

        float distanceFromPusher = Vector3.Distance(seeSharp.position, transform.position);
        float distanceFromJumper = Vector3.Distance(monty.position, transform.position);

        // Attack the player that is closer to the enemy.
        if (distanceFromJumper <= AttackRange && distanceFromPusher <= AttackRange)
        {
            if (distanceFromPusher < distanceFromJumper)
            {
                playerToAttack = seeSharp;
            }
            else
            {
                playerToAttack = monty;
            }
        }
        else if (distanceFromJumper <= AttackRange)
        {
            playerToAttack = monty;
        }
        else if (distanceFromPusher <= AttackRange)
        {
            playerToAttack = seeSharp;
        }

        return playerToAttack;
    }

    /// <summary>
    /// Cast a damaging spell at the location of the player closest to us.
    /// </summary>
    /// <param name="player">The player closest to us.</param>
    void AttackPlayer(Transform player)
    {
        // Register the fact that we can attack the player and that we want to chase if he escapes our range.
        isAggroed = true;

        FaceTarget(player);
        animator.SetTrigger("attack");

        if (canCastSpell)
        {
            // For now just cast spell at player position -- TODO maximise the positioning so that we damage the other player too
            // Use 0.1 for the Y so that it does not fight with the plane for rendering.
            GameObject spellInstance = Instantiate(spell, new Vector3(player.position.x, 0.1f, player.position.z), Quaternion.identity);

            // Extract the cooldown of the ability.
            Ability ability = spellInstance.GetComponentInChildren<Ability>();
            StartCoroutine(ResetCastTimer(ability.Cooldown));
        }
    }

    /// <summary>
    /// Chase the player closest to us.
    /// </summary>
    void ChaseClosestPlayer()
    {
        float distanceFromSeeSharp = Vector3.Distance(seeSharp.position, transform.position);
        float distanceFromMonty = Vector3.Distance(monty.position, transform.position);

        if(distanceFromSeeSharp <= distanceFromMonty)
        {
            agent.SetDestination(seeSharp.position);
        }
        else
        {
            agent.SetDestination(monty.position);
        }

        animator.SetTrigger("chase");
    }

    /// <summary>
    /// Rotate the GameObject towards the target.
    /// </summary>
    /// <param name="target">The target to rotate towards.</param>
    void FaceTarget(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    /// <summary>
    /// Allow spells to only be cast at a certain interval.
    /// </summary>
    /// <returns></returns>
    IEnumerator ResetCastTimer(int cooldown)
    {
        // Prevent casting until the spell has cooled down.
        canCastSpell = false;
        yield return new WaitForSecondsRealtime(cooldown);
        canCastSpell = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Ability ability = other.GetComponent<Ability>();

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
            print($"[WARNING] No damage on {transform.name} from {other.name}");
        }
    }

    void OnDrawGizmos()
    {
        DrawAggroRange();
    }

    // Kudos to the guy from http://codetuto.com/2015/06/drawing-a-circle-using-gizmos-in-unity3d/
    void DrawAggroRange()
    {
        Gizmos.color = color;
        float theta = 0;
        float x = AttackRange * Mathf.Cos(theta);
        float y = AttackRange * Mathf.Sin(theta);
        Vector3 pos = transform.position + new Vector3(x, 0, y);
        Vector3 newPos = pos;
        Vector3 lastPos = pos;
        for (theta = 0.1f; theta < Mathf.PI * 2; theta += 0.1f)
        {
            x = AttackRange * Mathf.Cos(theta);
            y = AttackRange * Mathf.Sin(theta);
            newPos = transform.position + new Vector3(x, 0, y);
            Gizmos.DrawLine(pos, newPos);
            pos = newPos;
        }
        Gizmos.DrawLine(pos, lastPos);
    }
}
