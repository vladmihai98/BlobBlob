using UnityEngine;

public class DragonController : MonoBehaviour
{
    [Header("Extra Stats")]
    [SerializeField] Transform firePoint;
    [SerializeField] GameObject fireBall;

    private float timeToFire = 0;
    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();    
    }

    /// <summary>
    /// Shoot a FireBall at the location of the player closest to us.
    /// </summary>
    /// <param name="player">The player closest to us.</param>
    private void Attack(Transform player)
    {
        if (Time.time >= timeToFire)
        {
            timeToFire = Time.time + 1 / controller.AttackSpeed;
            SpawnFireBall(player);
        }
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
            print("[ERROR] Fire point missing.");
        }

        return fireBallInstance;
    }
}
