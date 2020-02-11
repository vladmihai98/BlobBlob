using System.Collections;
using UnityEngine;

public class EnableSpellCollider : MonoBehaviour
{
    ParticleSystem ps;
    BoxCollider collider;

    /// <summary>
    /// Variable to prevent the collider from being enabled multiple times.
    /// </summary>
    bool colliderNotYetEnabled;

    void Start()
    {
        ps = transform.GetComponent<ParticleSystem>();
        collider = transform.GetComponent<BoxCollider>();
        collider.isTrigger = true;
        colliderNotYetEnabled = true;
    }

    void Update()
    {
        if(ps.isEmitting && colliderNotYetEnabled)
        {
            colliderNotYetEnabled = false;
            StartCoroutine(EnableCollider());
        }
    }

    /// <summary>
    /// Since the animation has a delay in spawning the particles that would deal damage,
    /// wait for that delay before turning on the collider, so that the user does not take damage earlier on.
    /// </summary>
    /// <returns></returns>
    IEnumerator EnableCollider()
    {
        yield return new WaitForSecondsRealtime(ps.main.startDelay.constant);
        collider.enabled = true;
        StartCoroutine(DisableCollider());
    }

    /// <summary>
    /// Disable collider once the animation is finished.
    /// </summary>
    /// <returns></returns>
    IEnumerator DisableCollider()
    {
        yield return new WaitForSecondsRealtime(ps.main.duration);
        collider.enabled = false;
    }
}
