using System.Collections;
using UnityEngine;

public class ConjurerController : MonoBehaviour
{
    [SerializeField] GameObject spell;

    private bool canAttack = true;

    /// <summary>
    /// Cast a damaging spell at the location of the player closest to us.
    /// </summary>
    /// <param name="player">The player closest to us.</param>
    private void Attack(Transform player)
    {
        if (canAttack)
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
    /// Allow spells to only be cast at a certain interval.
    /// </summary>
    /// <returns></returns>
    IEnumerator ResetCastTimer(int cooldown)
    {
        // Prevent casting until the spell has cooled down.
        canAttack = false;
        yield return new WaitForSecondsRealtime(cooldown);
        canAttack = true;
    }
}
