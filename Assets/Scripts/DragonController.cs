using UnityEngine;
using UnityEngine.AI;

public class DragonController : Character
{
    [Header("Extra Stats")]
    [SerializeField] Transform seeSharp;
    [SerializeField] Transform monty;
    [SerializeField] Transform firePoint;
    [SerializeField] Material material;
    [SerializeField] float bulletSpeed;
    [SerializeField] GameObject fireBall;

    private NavMeshAgent agent;
    private Color color;
    private float timeToFire = 0;

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
        if (isAggroed && didPlayerGoPastMe())
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
            if(isAggroed)
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
    private bool didPlayerGoPastMe()
    {
        if (initialPosition.position.z < monty.position.z ||
            initialPosition.position.z < seeSharp.position.z)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Buff base stats so that the character becomes more threatening for being ignored.
    /// </summary>
    private void BecomeEnraged()
    {
        
    }

    /// <summary>
    /// Retrieve the position of the nearest Player in range.
    /// </summary>
    /// <returns>The transform of the nearest enemy in range or null.</returns>
    private Transform isPlayerInAttackRange()
    {
        Transform playerToAttack = null;

        float distanceFromSeeSharp = Vector3.Distance(seeSharp.position, transform.position);
        float distanceFromMonty = Vector3.Distance(monty.position, transform.position);

        // Attack the player that is closer to the enemy.
        if (distanceFromMonty <= AttackRange && distanceFromSeeSharp <= AttackRange)
        {
            if (distanceFromSeeSharp < distanceFromMonty)
            {
                playerToAttack = seeSharp;
            }
            else
            {
                playerToAttack = monty;
            }
        }
        else if (distanceFromMonty <= AttackRange)
        {
            playerToAttack = monty;
        }
        else if (distanceFromSeeSharp <= AttackRange)
        {
            playerToAttack = seeSharp;
        }

        if(playerToAttack == null)
        {
            if(distanceFromMonty <= AttackRange + 10 || 
               distanceFromSeeSharp <= AttackRange + 10)
            {
                animator.SetTrigger("wake");
            }
        }

        return playerToAttack;
    }

    /// <summary>
    /// Shoot a FireBall at the location of the player closest to us.
    /// </summary>
    /// <param name="player">The player closest to us.</param>
    private void AttackPlayer(Transform player)
    {
        // Register the fact that we can attack the player and that we want to chase if he escapes our range.
        isAggroed = true;

        FaceTarget(player);
        animator.SetTrigger("attack");

        if (Time.time >= timeToFire)
        {
            timeToFire = Time.time + 1 / AttackSpeed;
            GameObject bullet = SpawnFireBall(player);
        }
    }

    /// <summary>
    /// Chase the player closest to us.
    /// </summary>
    void ChaseClosestPlayer()
    {
        float distanceFromSeeSharp = Vector3.Distance(seeSharp.position, transform.position);
        float distanceFromMonty = Vector3.Distance(monty.position, transform.position);

        if (distanceFromSeeSharp <= distanceFromMonty)
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
    /// Spawn the FireBall VFX and rotate it towards the target.
    /// </summary>
    /// <param name="player">The player who to shoot towards.</param>
    /// <returns>The FireBall instance.</returns>
    GameObject SpawnFireBall(Transform player)
    {
        GameObject fireBallInstance = null;

        if (firePoint != null)
        {
            Vector3 direction = player.position - firePoint.position;
            direction.y += 5f; // add a bit of height since we're targeting the pivot on the Y axis which is at the base of model.
            Quaternion newQuat = Quaternion.LookRotation(direction);
            fireBallInstance = Instantiate(fireBall, firePoint.position, newQuat);
        }
        else
        {
            print("Fire point missing.");
        }

        return fireBallInstance;
    }

    private void OnTriggerEnter(Collider other)
    {
        Ability ability = other.GetComponent<Ability>();

        // Ignore friendly-fire, e.g. abilities that should damage enemies and not our friends.
        if (ability.TypeOfTarget == Ability.TargetType.Player)
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
