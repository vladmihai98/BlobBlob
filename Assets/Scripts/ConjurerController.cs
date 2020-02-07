using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConjurerController : Character
{
    [Header("Extra Stats")]
    [SerializeField] Transform pusher;
    [SerializeField] Transform jumper;
    [SerializeField] GameObject spell;
    [SerializeField] Material material;

    private bool canCastSpell = true;
    private Color color;

    // Start is called before the first frame update
    void Start()
    {
        color = material.color;
        currentHealth = MaxHealth;
    }

    // Update is called once per frame
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
        if (playerToAttack != null)
        {
            AttackPlayer(playerToAttack);
        }
    }

    bool didPlayerGoPastMe()
    {
        return false;
    }

    void BecomeEnraged()
    {

    }

    Transform isPlayerInAttackRange()
    {
        Transform playerToAttack = null;

        float distanceFromPusher = Vector3.Distance(pusher.position, transform.position);
        float distanceFromJumper = Vector3.Distance(jumper.position, transform.position);

        // Attack the player that is closer to the enemy.
        if (distanceFromJumper <= AttackRange && distanceFromPusher <= AttackRange)
        {
            if (distanceFromPusher < distanceFromJumper)
            {
                playerToAttack = pusher;
            }
            else
            {
                playerToAttack = jumper;
            }
        }
        else if (distanceFromJumper <= AttackRange)
        {
            playerToAttack = jumper;
        }
        else if (distanceFromPusher <= AttackRange)
        {
            playerToAttack = pusher;
        }

        return playerToAttack;
    }

    void AttackPlayer(Transform player)
    {
        print($"attacking {player.name}");

        if (canCastSpell)
        {
            print($"instantiating + {canCastSpell}");

            // For now just cast spell at player position
            // Use 0.1 for the Y so that it does not fight with the plane for rendering.
            GameObject spellInstance = Instantiate(spell, new Vector3(player.position.x, 0.1f, player.position.z), Quaternion.identity);

            // Give damage to the ability.
            Ability ability = spellInstance.GetComponentInChildren<Ability>();
            ability.AbilityPower = AbilityPower;

            StartCoroutine(ResetCastTimer());
        }
    }

    IEnumerator ResetCastTimer()
    {
        print("falsing");
        canCastSpell = false;

        yield return new WaitForSecondsRealtime(CastingSpeed);

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
