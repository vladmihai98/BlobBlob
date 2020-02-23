﻿using UnityEngine;

public class SeeSharpShooter : MonoBehaviour
{
    [SerializeField] Transform firePoint;
    [SerializeField][Tooltip("VFX for bullet")] GameObject vfx;

    [Tooltip("Distance to capture raycasthit.")]
    [SerializeField] float maximumLength = 5000f;

    [SerializeField] SeeSharpController controller;

    private Camera camera;
    private float timeToFire = 0f;
    private float fireRate;

    void Start()
    {
        camera = Camera.main;
        fireRate = controller.AttackSpeed;
    }

    void Update()
    {
        Quaternion newRotation = Quaternion.identity;

        if (camera != null)
        {
            RaycastHit hit;
            var mousePosition = Input.mousePosition;
            Ray mouseRay = camera.ScreenPointToRay(mousePosition);

            if (Physics.Raycast(mouseRay.origin, mouseRay.direction, out hit, maximumLength))
            {
                newRotation = CalculateNewRotation(firePoint, hit.point);
            }
            else
            {
                Vector3 position = mouseRay.GetPoint(maximumLength);
                newRotation = CalculateNewRotation(firePoint, position);
            }
        }

        if (Input.GetMouseButton(0) && Time.time >= timeToFire)
        {
            timeToFire = Time.time + 1 / fireRate;
            SpawnVfx(newRotation);

            if(controller.GetVelocity() == Vector3.zero)
            {
                controller.animator.SetBool("shoot", true);
            }
        }
        else if(!Input.GetMouseButton(0) || controller.GetVelocity() != Vector3.zero)
        {
            controller.animator.SetBool("shoot", false);
        }
    }

    Quaternion CalculateNewRotation(Transform target, Vector3 destination)
    {
        Vector3 direction = destination - target.position;
        Quaternion rotation = Quaternion.LookRotation(direction);

        return Quaternion.Lerp(target.rotation, rotation, 1);
    }

    void SpawnVfx(Quaternion newRotation)
    {
        GameObject bullet;

        if (firePoint != null)
        {
            bullet = Instantiate(vfx, firePoint.position, Quaternion.identity);
            bullet.transform.localRotation = newRotation;

            // Give damage to the bullet instance.
            Ability ability = bullet.GetComponent<Ability>();
            ability.AttackDamage = controller.AttackDamage;
        }
        else
        {
            print("Fire point missing.");
        }
    }
}