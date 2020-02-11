using System.Collections.Generic;
using UnityEngine;

public class RangerController : Character
{
    [Header("Extra Stats")]
    [SerializeField] Transform seeSharp;
    [SerializeField] Transform monty;
    [SerializeField] Transform firePoint;
    [SerializeField] Material material;
    [SerializeField] float bulletSpeed;
    [SerializeField] List<GameObject> bullets = new List<GameObject>();

    private GameObject effectToSpawn;
    private Color color;
    private float timeToFire = 0;

    // Start is called before the first frame update
    void Start()
    {
        color = material.color;
        effectToSpawn = bullets[0];
        currentHealth = MaxHealth;
    }

    void Update()
    {
        /*
         * get player from world
         * get player position
         * if(playerPosition is past transform.position)
         * {
         *      // become enraged
         *      // more damage, increased AS, increased Casting Speed
         *      // increased MS
         *      // and follow the enemy forever = Video 172 from Udemy
         * }
         */
        if (didPlayerGoPastMe())
        {
            BecomeEnraged();
        }

        Transform playerToAttack = isPlayerInAttackRange();
        if (playerToAttack)
        {
            AttackPlayer(playerToAttack);
        }
    }

    private bool didPlayerGoPastMe()
    {
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

    private void AttackPlayer(Transform player)
    {
        FaceTarget(player);

        if(Time.time >= timeToFire)
        {
            timeToFire = Time.time + 1 / AttackSpeed;
            GameObject bullet = SpawnBullet(player);

            // Give damage to the bullet instance.
            Ability ability = bullet.GetComponent<Ability>();
            ability.AttackDamage = AttackDamage;
            //MoveBullet(player.position, bullet);
        }
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
    /// Spawn the bullet VFX and rotate it towards the target.
    /// </summary>
    /// <param name="player"></param>
    /// <returns>The bullet instance.</returns>
    GameObject SpawnBullet(Transform player)
    {
        GameObject bullet = null;

        if (firePoint != null)
        {
            Vector3 direction = player.position - firePoint.position;
            Quaternion newQuat = Quaternion.LookRotation(direction);

            bullet = Instantiate(effectToSpawn, firePoint.position, newQuat);
        }
        else
        {
            print("Fire point missing.");
        }

        return bullet;
    }

    void MoveBullet(Vector3 position, GameObject bullet)
    {
        if (bulletSpeed >= 0)
        {
            bullet.transform.position += transform.forward * (bulletSpeed * Time.deltaTime);
        }
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
