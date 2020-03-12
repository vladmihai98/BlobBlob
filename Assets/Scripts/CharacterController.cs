using UnityEngine;
using UnityEngine.AI;

public class CharacterController : Character
{
    [Header("Extra Stats")]
    [SerializeField] Material material;
    [SerializeField] bool isDragon;
    [SerializeField] bool isBerserker;

    protected NavMeshAgent agent;
    private Canvas healthHud;
    private Vector3 newHudPosition;
    private BoxCollider collider;
    private Transform seeSharp;
    private Transform monty;
    private Color color;
    private int aggroRange = 0;

    void Start()
    {
        // Initialise inherited variables.
        initialPosition = transform;
        currentHealth = MaxHealth;

        // Get references to the players.
        seeSharp = FindObjectOfType<SeeSharpController>().transform;
        monty = FindObjectOfType<MontyController>().transform;

        color = material.color;
        collider = GetComponent<BoxCollider>();
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = AttackRange;

        if(isBerserker)
        {
            aggroRange = AttackRange + 50;
        }

        if(isDragon)
        {
            healthHud = GetComponentInChildren<Canvas>();
            Vector3 hudLocalPos = healthHud.transform.localPosition;
            newHudPosition = new Vector3(hudLocalPos.x, hudLocalPos.y + 3f, hudLocalPos.z);
        }
    }

    public override void Interact()
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
            if (isAggroed || (aggroRange > 0 && isPlayerInAggroRange()))
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
    void BecomeEnraged()
    {

    }

    /// <summary>
    /// Retrieve the position of the nearest Player in Attack Range.
    /// </summary>
    /// <returns>The transform of the nearest enemy in Attack Range or null.</returns>
    Transform isPlayerInAttackRange()
    {
        return isPlayerInRange(AttackRange);
    }

    /// <summary>
    /// Check if there is at least a player in AggroRange.
    /// </summary>
    /// <returns>True if there is a player in AggroRange, false otherwise.</returns>
    bool isPlayerInAggroRange()
    {
        if(isPlayerInRange(aggroRange))
        {
            return true;
        }

        return false;
    }

    Transform isPlayerInRange(int rangeThreshold)
    {
        Transform playerToAttack = null;

        float distanceFromSeeSharp = Vector3.Distance(seeSharp.position, transform.position);
        float distanceFromMonty = Vector3.Distance(monty.position, transform.position);

        // Attack the player that is closer to the enemy.
        if (distanceFromMonty <= rangeThreshold && distanceFromSeeSharp <= rangeThreshold)
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
        else if (distanceFromMonty <= rangeThreshold)
        {
            playerToAttack = monty;
        }
        else if (distanceFromSeeSharp <= rangeThreshold)
        {
            playerToAttack = seeSharp;
        }

        if (isDragon && playerToAttack == null)
        {
            if (distanceFromMonty <= rangeThreshold + 30 ||
               distanceFromSeeSharp <= rangeThreshold + 30)
            {
                animator.SetTrigger("wake");
            }
        }

        return playerToAttack;
    }

    /// <summary>
    /// Attempt to damage the player closest to us.
    /// </summary>
    /// <param name="player">The player closest to us.</param>
    public virtual void AttackPlayer(Transform player)
    {
        // Register the fact that we can attack the player and that we want to chase if he escapes our range.
        isAggroed = true;
        FaceTarget(player);
        animator.SetTrigger("attack");

        SendMessage("Attack", player);
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

        if(isDragon)
        {
            // Raise the collider since the model of the dragon raises to fly for chasing.
            Vector3 colliderCenter = collider.center;
            colliderCenter.y = 3.3f;
            collider.center = colliderCenter;

            // Raise the HUD as well.
            healthHud.transform.localPosition = Vector3.Lerp(healthHud.transform.localPosition, newHudPosition, 0.1f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Ability ability = other.GetComponent<Ability>();
        if(ability)
        {
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
        float x = aggroRange * Mathf.Cos(theta);
        float y = aggroRange * Mathf.Sin(theta);
        Vector3 pos = transform.position + new Vector3(x, 0, y);
        Vector3 newPos = pos;
        Vector3 lastPos = pos;
        for (theta = 0.1f; theta < Mathf.PI * 2; theta += 0.1f)
        {
            x = aggroRange * Mathf.Cos(theta);
            y = aggroRange * Mathf.Sin(theta);
            newPos = transform.position + new Vector3(x, 0, y);
            Gizmos.DrawLine(pos, newPos);
            pos = newPos;
        }
        Gizmos.DrawLine(pos, lastPos);
    }
}
