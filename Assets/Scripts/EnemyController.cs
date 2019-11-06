using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] float attackRange = 5f;
    [SerializeField] Transform pusher;
    [SerializeField] Transform jumper;
    [SerializeField] GameObject spell;

    [Tooltip("Delay between casting again in s")]
    [SerializeField] float castingSpeed = 5;

    private bool canCastSpell = true;

    // Start is called before the first frame update
    void Start()
    {
        
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
        if(didPlayerGoPastMe())
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
        if(distanceFromJumper <= attackRange && distanceFromPusher <= attackRange)
        {
            if(distanceFromPusher < distanceFromJumper)
            {
                playerToAttack = pusher;
            }
            else
            {
                playerToAttack = jumper;
            }
        }
        else if(distanceFromJumper <= attackRange)
        {
            playerToAttack = jumper;
        }
        else if(distanceFromPusher <= attackRange)
        {
            playerToAttack = pusher;
        }

        return playerToAttack;
    }

    void AttackPlayer(Transform player)
    {
        // TODO attack player - we're friendly for now >:)
        print($"attacking {player.name}");

        if(canCastSpell)
        {
            print($"instantiating + {canCastSpell}");

            // For now just cast spell at player position
            Instantiate(spell, player.position, Quaternion.identity);

            StartCoroutine(ResetCastTimer());
        }
    }

    IEnumerator ResetCastTimer()
    {
        print("falsing");
        canCastSpell = false;

        yield return new WaitForSecondsRealtime(castingSpeed);

        canCastSpell = true;
    }

    void OnDrawGizmos()
    {
        DrawAggroRange();
    }

    // Kudos to the guy from http://codetuto.com/2015/06/drawing-a-circle-using-gizmos-in-unity3d/
    void DrawAggroRange()
    {
        Gizmos.color = Color.red;
        float theta = 0;
        float x = attackRange * Mathf.Cos(theta);
        float y = attackRange * Mathf.Sin(theta);
        Vector3 pos = transform.position + new Vector3(x, 0, y);
        Vector3 newPos = pos;
        Vector3 lastPos = pos;
        for (theta = 0.1f; theta < Mathf.PI * 2; theta += 0.1f)
        {
            x = attackRange * Mathf.Cos(theta);
            y = attackRange * Mathf.Sin(theta);
            newPos = transform.position + new Vector3(x, 0, y);
            Gizmos.DrawLine(pos, newPos);
            pos = newPos;
        }
        Gizmos.DrawLine(pos, lastPos);
    }
}
