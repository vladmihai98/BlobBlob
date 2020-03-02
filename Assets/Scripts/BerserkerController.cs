using System.Collections;
using UnityEngine;
using static Character;

public class BerserkerController : MonoBehaviour
{
    private CharacterController controller;
    private bool canAttack = true;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    /// <summary>
    /// Swing the axe at the player, causing to just inflict damage on the target.
    /// To dodge, the player should try to kite back.
    /// </summary>
    /// <param name="player">The player closest to us.</param>
    void Attack(Transform player)
    {
        if(canAttack)
        {
            canAttack = false;

            SeeSharpController seeSharpController = player.GetComponent<SeeSharpController>();
            MontyController montyController = player.GetComponent<MontyController>();

            if (seeSharpController)
            {
                seeSharpController.TakeHit(controller.AttackDamage, Resistance.UseArmor);
            }

            if (montyController)
            {
                montyController.TakeHit(controller.AttackDamage, Resistance.UseArmor);
            }

            StartCoroutine(ResetTimer());
        }
    }

    IEnumerator ResetTimer()
    {
        yield return new WaitForSecondsRealtime(controller.AttackSpeed);
        canAttack = true;
    }
}
