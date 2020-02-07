using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableSpellCollider : MonoBehaviour
{
    ParticleSystem ps;
    BoxCollider collider;
    bool colliderNotYetEnabled;

    // Start is called before the first frame update
    void Start()
    {
        ps = transform.GetComponent<ParticleSystem>();
        collider = transform.GetComponent<BoxCollider>();
        collider.isTrigger = true;
        colliderNotYetEnabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(ps.isEmitting && colliderNotYetEnabled)
        {
            print("1");
            colliderNotYetEnabled = false;
            StartCoroutine(EnableCollider());
        }
    }

    // Since the animation has a delay in spawning the particles that would deal damage
    // Wait for that delay before turning on the collider, so that the user does not take damage earlier on.
    IEnumerator EnableCollider()
    {
        yield return new WaitForSecondsRealtime(ps.main.startDelay.constant);

        print("2");
        collider.enabled = true;
        StartCoroutine(DisableCollider());
    }

    IEnumerator DisableCollider()
    {
        yield return new WaitForSecondsRealtime(ps.main.duration);

        print("3");
        collider.enabled = false;
    }
}
