using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AssassinController : Character
{
    [Header("Extra Stats")]
    [SerializeField] Transform seeSharp;
    [SerializeField] Transform monty;
    [SerializeField] Material material;

    private Color color;
    private NavMeshAgent agent;
    private bool canAttack = true;

    private MontyController montyCtrl;
    private SeeSharpController seeSharpCtrl;

    void Start()
    {
        // Initialise inherited variables.
        initialPosition = transform;
        currentHealth = MaxHealth;

        color = material.color;
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = AttackRange;

        montyCtrl = monty.GetComponent<MontyController>();
        seeSharpCtrl = seeSharp.GetComponent<SeeSharpController>();
    }

    void LateUpdate()
    {
        if(isAlive)
        {
            anotherGo();
        }
        //ChooseNextAction();
        //if (isAggroed && didPlayerIgnoreMe())
        //{
        //    BecomeEnraged();
        //}

        //Transform playerToAttack = isPlayerInAttackRange();
        //if (playerToAttack)
        //{
        //    AttackPlayer(playerToAttack);
        //}
        //else
        //{
        //    if (isAggroed)
        //    {
        //        ChaseClosestPlayer();
        //    }
        //}
    }

    void ChooseNextAction()
    {
        if(isAggroed)
        {
            if(didPlayerIgnoreMe())
            {
                if(canAttack)
                {
                    int montyHealth = montyCtrl.GetCurrentHealth();
                    int seeSharpHealth = seeSharpCtrl.GetCurrentHealth();

                    print("I SHALL NOT BE IGNORED!!!");

                    if(montyHealth < seeSharpHealth)
                    {
                        AttackPlayer(monty, false);
                    }
                    else
                    {
                        AttackPlayer(seeSharp, false);
                    }
                }
            }
            else
            {
                if(canAttack)
                {
                    Transform playerToAttack = isPlayerInAttackRange();
                    AttackPlayer(playerToAttack);
                }
                else
                {
                    // Attempt to dodge incoming bullets. 
                    // The Energy Slash is meant to be undodgeable so we do not track that.
                    Dodge();
                }
            }
        }
        else
        {
            Transform playerToAttack = isPlayerInAttackRange();
            if(playerToAttack)
            {
                AttackPlayer(playerToAttack);
            }
        }
    }

    void anotherGo()
    {
        Transform playerToAttack = isPlayerInAttackRange();

        if(isAggroed)
        {
            if(canAttack)
            {
                if(didPlayerIgnoreMe())
                {
                    int montyHealth = montyCtrl.GetCurrentHealth();
                    int seeSharpHealth = seeSharpCtrl.GetCurrentHealth();

                    print("I SHALL NOT BE IGNORED!!!");

                    if (montyHealth < seeSharpHealth)
                    {
                        AttackPlayer(monty, false);
                    }
                    else
                    {
                        AttackPlayer(seeSharp, false);
                    }
                }
                else
                {
                    if (playerToAttack)
                    {
                        AttackPlayer(playerToAttack);
                    }
                }
            }
            else
            {
                //Dodge();
            }
        }
        else
        {
            if(playerToAttack)
            {
                AttackPlayer(playerToAttack);
            }
        }
    }

    void Dodge()
    {
        // Get instance of bullet.
        // Calculate direction and cast a ray thru that.. if it hits us step aside.
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
    /// Retrieve the position of the nearest Player in range.
    /// </summary>
    /// <returns>The transform of the nearest enemy in range or null.</returns>
    Transform isPlayerInAttackRange()
    {
        Transform playerToAttack = null;

        var montyController = monty.GetComponent<MontyController>();
        var seeSharpController = seeSharp.GetComponent<SeeSharpController>();

        float distanceFromSeeSharp = Vector3.Distance(seeSharp.position, transform.position);
        float distanceFromMonty = Vector3.Distance(monty.position, transform.position);

        // Attack the players if they are in range.
        if (distanceFromMonty <= AttackRange && distanceFromSeeSharp <= AttackRange)
        {
            // Attack the player that has the fewest HealthPoints.
            if (montyController.GetCurrentHealth() < seeSharpController.GetCurrentHealth())
            {
                playerToAttack = monty;
            }
            else
            {
                playerToAttack = seeSharp;
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

        return playerToAttack;
    }

    /// <summary>
    /// Cast a damaging spell at the location of the player closest to us.
    /// </summary>
    /// <param name="player">The player closest to us.</param>
    void AttackPlayer(Transform player, bool resetPosition = true)
    {
        // Register the fact that we can attack the player and that we want to chase if he escapes our range.
        isAggroed = true;

        FaceTarget(player);

        if (canAttack)
        {
            canAttack = false;

            Vector3 positionReset = transform.position;

            transform.position = new Vector3(player.position.x, player.position.y, player.position.z + 5f);

            if (resetPosition)
            {
                StartCoroutine(PrepareAndAttack(positionReset, player));
            }
            else
            {
                StartCoroutine(PrepareAndAttack(player));
            }
        }
    }

    IEnumerator PrepareAndAttack(Transform player)
    {
        yield return new WaitForSecondsRealtime(1f);
        Vector3 newPosition = new Vector3(player.position.x, player.position.y, player.position.z - 5f);
        transform.position = newPosition;
        FaceTarget(player);
        animator.SetBool("attack", true);

        var ctrl = player.GetComponent<SeeSharpController>();
        var ctrl1 = player.GetComponent<MontyController>();

        if (ctrl)
        {
            ctrl.TakeHit(AttackDamage, Resistance.UseArmor);
        }
        if (ctrl1)
        {
            //ctrl1.SetMovementSpeed(ctrl1.GetMovementSpeed() / 2);
            ctrl1.TakeHit(AttackDamage, Resistance.UseArmor);
        }

        StartCoroutine(ResetAttack());
    }

    IEnumerator PrepareAndAttack(Vector3 positionToResetTo, Transform player)
    {
        yield return new WaitForSecondsRealtime(1f);
        Vector3 newPosition = new Vector3(player.position.x, player.position.y, player.position.z - 5f);
        transform.position = newPosition;
        FaceTarget(player);
        animator.SetBool("attack", true);

        var ctrl = player.GetComponent<SeeSharpController>();
        var ctrl1 = player.GetComponent<MontyController>();

        if (ctrl)
        {
            ctrl.TakeHit(AttackDamage, Resistance.UseArmor);
        }
        if(ctrl1)
        {
            //ctrl1.SetMovementSpeed(ctrl1.GetMovementSpeed() / 2);
            ctrl1.TakeHit(AttackDamage, Resistance.UseArmor);
        }

        StartCoroutine(ResetPosition(positionToResetTo));
    }

    IEnumerator ResetPosition(Vector3 positionToResetTo)
    {
        yield return new WaitForSecondsRealtime(0.5f);
        transform.position = positionToResetTo;
        animator.SetBool("attack", false);
        StartCoroutine(ResetAttack());
    }

    IEnumerator ResetAttack()
    {
        yield return new WaitForSecondsRealtime(AttackSpeed);
        canAttack = true;
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
