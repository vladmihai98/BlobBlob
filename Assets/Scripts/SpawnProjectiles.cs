using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnProjectiles : MonoBehaviour
{
    [SerializeField] Transform firePoint;
    [SerializeField] List<GameObject> vfx = new List<GameObject>();
    [SerializeField] float fireRate;

    [Tooltip("Reference to object which rotates to mouse and fires vfx")]
    [SerializeField] RotateToMouse rotateToMouse;

    private GameObject effectToSpawn;
    private float timeToFire = 0;
    private PusherController controller;

    // Start is called before the first frame update
    void Start()
    {
        effectToSpawn = vfx[0];
        controller = gameObject.GetComponent<PusherController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0) && Time.time >= timeToFire)
        {
            timeToFire = Time.time + 1 / fireRate;
            SpawnVfx();
        }
    }

    void SpawnVfx()
    {
        GameObject bullet = null;
        var position = firePoint.position;

        if (firePoint != null)
        {
            bullet = Instantiate(effectToSpawn, position, Quaternion.identity);

            if(rotateToMouse)
            {
                bullet.transform.localRotation = rotateToMouse.GetRotation();
            }

            Ability ability = bullet.GetComponent<Ability>();
            ability.AttackDamage = controller.AttackDamage;
        }
        else
        {
            print("Fire point missing.");
        }
    }
}
