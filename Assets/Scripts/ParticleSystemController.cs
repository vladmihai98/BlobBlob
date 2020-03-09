using UnityEngine;

public class ParticleSystemController : MonoBehaviour
{
    private Ability ability;

    void Start()
    {
        ability = GetComponent<Ability>();
        Destroy(gameObject, ability.Duration);
    }

    void Update()
    {
        if (ability.Speed >= 0)
        {
            transform.position += transform.forward * (ability.Speed * Time.deltaTime);
        }
    }
}
