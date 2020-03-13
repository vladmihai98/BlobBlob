using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AssassinController : Character
{
    [Header("Extra Stats")]
    [SerializeField] Material material;
    [SerializeField] LayerMask assassinMask;

    private Color color;
    private NavMeshAgent agent;
    private Rigidbody rigidbody;
    private bool canAttack = true;
    private int timesDodged = 0;
    private const int DODGE_CAP = 2;

    private GameController gameController;
    private MontyController montyController;
    private SeeSharpController seeSharpController;

    void Start()
    {
        // Initialise inherited variables.
        initialPosition = transform;
        currentHealth = MaxHealth;

        color = material.color;
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = AttackRange;
        rigidbody = GetComponent<Rigidbody>();

        // Get references to the players and to the game controller.
        gameController = FindObjectOfType<GameController>();
        montyController = FindObjectOfType<MontyController>();
        seeSharpController = FindObjectOfType<SeeSharpController>();
    }

    void LateUpdate()
    {
        if(isAlive)
        {
            ChooseNextAction();
        }
    }

    void ChooseNextAction()
    {
        Transform playerToAttack = isPlayerInAttackRange();

        if(isAggroed)
        {
            if(canAttack)
            {
                if(isEnraged || didPlayerIgnoreMe())
                {
                    isEnraged = true;

                    int montyHealth = gameController.IsMontyAlive() ? montyController.GetCurrentHealth() : 0;
                    int seeSharpHealth = gameController.IsSeeSharpAlive() ? seeSharpController.GetCurrentHealth() : 0;
                    if (gameController.IsMontyAlive() && montyHealth < seeSharpHealth)
                    {
                        AttackPlayer(montyController.transform, false);
                    }
                    else if(gameController.IsSeeSharpAlive())
                    {
                        AttackPlayer(seeSharpController.transform, false);
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
            else if(timesDodged < DODGE_CAP)
            {
                Dodge();
                ChaseClosestPlayer();
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

    /// <summary>
    /// Retrieve the position of the nearest Player in range.
    /// </summary>
    /// <returns>The transform of the nearest enemy in range or null.</returns>
    Transform isPlayerInAttackRange()
    {
        Transform playerToAttack = null;
        float distanceFromSeeSharp = float.MaxValue;
        float distanceFromMonty = float.MaxValue;
        if(gameController.IsSeeSharpAlive())
        {
            distanceFromSeeSharp = Vector3.Distance(seeSharpController.transform.position, transform.position);
        }
        if (gameController.IsMontyAlive())
        {
            distanceFromMonty = Vector3.Distance(montyController.transform.position, transform.position);
        }

        // Attack the players if they are in range.
        if (distanceFromMonty <= AttackRange && distanceFromSeeSharp <= AttackRange)
        {
            // Attack the player that has the fewest HealthPoints.
            if (gameController.IsMontyAlive())
            {
                if(gameController.IsSeeSharpAlive())
                {
                    if(montyController.GetCurrentHealth() < seeSharpController.GetCurrentHealth())
                    {
                        playerToAttack = montyController.transform;
                    }
                    else
                    {
                        playerToAttack = seeSharpController.transform;
                    }
                }
                else
                {
                    playerToAttack = montyController.transform;
                }
            }
            else if (gameController.IsSeeSharpAlive())
            {
                playerToAttack = seeSharpController.transform;
            }
        }
        else if (gameController.IsMontyAlive() && distanceFromMonty <= AttackRange)
        {
            playerToAttack = montyController.transform;
        }
        else if (gameController.IsSeeSharpAlive() && distanceFromSeeSharp <= AttackRange)
        {
            playerToAttack = seeSharpController.transform;
        }
        return playerToAttack;
    }

    /// <summary>
    /// For convenience the enemies are placed facing Z-.
    /// So the players ignore the enemies when they have gone past the initial position plus range in the Z+ direction.
    /// </summary>
    /// <returns>True if player ignored enemy, false otherwise.</returns>
    bool didPlayerIgnoreMe()
    {
        if ((gameController.IsMontyAlive() && initialPosition.position.z < montyController.transform.position.z) ||
            (gameController.IsSeeSharpAlive() && initialPosition.position.z < seeSharpController.transform.position.z))
        {
            isEnraged = true;
            BecomeEnraged();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Buff base stats so that the character becomes more threatening for being ignored.
    /// </summary>
    void BecomeEnraged()
    {
        Armor += 10;
        AttackDamage += 75;
        MagicResist += 10;
    }

    /// <summary>
    /// Cast a damaging spell at the location of the player closest to us.
    /// </summary>
    /// <param name="player">The player closest to us.</param>
    void AttackPlayer(Transform player, bool resetPosition = true)
    {
        // Register the fact that we can attack the player and that we want to chase if he escapes our range.
        isAggroed = true;

        // Reset the dodge count.
        timesDodged = 0;

        // Face the target even if we cannot attack it.
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

    IEnumerator PrepareAndAttack(Vector3 positionToResetTo, Transform player)
    {
        yield return new WaitForSecondsRealtime(1f);
        Vector3 newPosition = new Vector3(player.position.x, player.position.y, player.position.z - 5f);
        transform.position = newPosition;
        FaceTarget(player);
        animator.SetBool("attack", true);

        // Check if the player is still alive.
        SeeSharpController ssCtrl = null;
        MontyController mCtrl = null;
        if (gameController.IsSeeSharpAlive())
        {
            ssCtrl = player.GetComponent<SeeSharpController>();
        }
        if (gameController.IsMontyAlive())
        {
            mCtrl = player.GetComponent<MontyController>();
        }

        // Attack the player if alive.
        if (ssCtrl)
        {
            ssCtrl.TakeHit(AttackDamage, Resistance.UseArmor);
        }
        if (mCtrl)
        {
            //ctrl1.SetMovementSpeed(ctrl1.GetMovementSpeed() / 2);
            mCtrl.TakeHit(AttackDamage, Resistance.UseArmor);
        }

        StartCoroutine(ResetPosition(positionToResetTo));
    }

    IEnumerator PrepareAndAttack(Transform player)
    {
        yield return new WaitForSecondsRealtime(1f);
        Vector3 newPosition = new Vector3(player.position.x, player.position.y, player.position.z - 5f);
        transform.position = newPosition;
        FaceTarget(player);
        animator.SetBool("attack", true);

        // Check if the player is still alive.
        SeeSharpController ssCtrl = null;
        MontyController mCtrl = null;
        if (gameController.IsSeeSharpAlive())
        {
            ssCtrl = player.GetComponent<SeeSharpController>();
        }
        if (gameController.IsMontyAlive())
        {
            mCtrl = player.GetComponent<MontyController>();
        }

        // Attack the player if alive.
        if (ssCtrl)
        {
            ssCtrl.TakeHit(AttackDamage, Resistance.UseArmor);
        }
        if (mCtrl)
        {
            //ctrl1.SetMovementSpeed(ctrl1.GetMovementSpeed() / 2);
            mCtrl.TakeHit(AttackDamage, Resistance.UseArmor);
        }

        StartCoroutine(ResetAttack());
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
        Transform closestPlayer = GetClosestPlayer();
        if (closestPlayer)
        {
            agent.SetDestination(closestPlayer.position);
        }
        else
        {
            agent.isStopped = true;
        }
        //animator.SetTrigger("chase");
    }

    Transform GetClosestPlayer()
    {
        Transform closestPlayer = null;
        float distanceFromSeeSharp = float.MaxValue;
        float distanceFromMonty = float.MaxValue;
        if (gameController.IsSeeSharpAlive())
        {
            distanceFromSeeSharp = Vector3.Distance(seeSharpController.transform.position, transform.position);
        }
        if (gameController.IsMontyAlive())
        {
            distanceFromMonty = Vector3.Distance(montyController.transform.position, transform.position);
        }

        if (gameController.IsMontyAlive())
        {
            if(gameController.IsSeeSharpAlive())
            {
                if(distanceFromMonty <= distanceFromSeeSharp)
                {
                    closestPlayer = montyController.transform;
                }
                else
                {
                    closestPlayer = seeSharpController.transform;
                }
            }
            else
            {
                closestPlayer = montyController.transform;
            }
        }
        else if (gameController.IsSeeSharpAlive())
        {
            closestPlayer = seeSharpController.transform;
        }
        return closestPlayer;
    }

    void Dodge()
    {
        BulletController bulletController = FindObjectOfType<BulletController>();

        // If there is a bullet in the scene (i.e. the player is shooting).
        if (bulletController)
        {
            // Get its direction.
            Vector3 direction = bulletController.GetDirection();

            // And find out if that would hit us.
            RaycastHit hit;
            if (Physics.Raycast(bulletController.transform.position, direction, out hit, 5000, assassinMask))
            {
                timesDodged++;

                float randomValue = Random.value;
                Vector3 positionAlteration;

                if (randomValue <= 0.25f)
                {
                    positionAlteration = Vector3.forward;
                }
                else if (randomValue <= 0.5f)
                {
                    positionAlteration = Vector3.right;
                }
                else if (randomValue <= 0.75f)
                {
                    positionAlteration = Vector3.back;
                }
                else
                {
                    positionAlteration = Vector3.left;
                }

                rigidbody.MovePosition(GetClosestPlayer().position + positionAlteration * 10f);
            }
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
