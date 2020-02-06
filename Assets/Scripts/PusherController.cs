using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PusherController : Character
{
    private Rigidbody rigidbody;
    private Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        currentHealth = MaxHealth;
    }

    public override void Interact()
    {
        Move();
    }

    public override void Move()
    {
        ProcessInput();

        rigidbody.MovePosition(transform.position + (velocity * MovementSpeed * Time.deltaTime));
    }

    void ProcessInput()
    {
        if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.LeftArrow))
        {
            velocity = Vector3.forward + Vector3.left;
        }
        else if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.RightArrow))
        {
            velocity = Vector3.forward + Vector3.right;
        }
        else if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftArrow))
        {
            velocity = Vector3.back + Vector3.left;
        }
        else if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.RightArrow))
        {
            velocity = Vector3.back + Vector3.right;
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            velocity = Vector3.forward;
        }
        else if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            velocity = Vector3.zero;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            velocity = Vector3.left;
        }
        else if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            velocity = Vector3.zero;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            velocity = Vector3.back;
        }
        else if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            velocity = Vector3.zero;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            velocity = Vector3.right;
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            velocity = Vector3.zero;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Ability ability = other.GetComponent<Ability>();

        if(ability.AttackDamage > 0)
        {
            TakeDamage(ability.AttackDamage, Resistance.UseArmor);
        }
        else if(ability.AbilityPower > 0)
        {
            TakeDamage(ability.AbilityPower, Resistance.UseMagicResist);
        }
        else
        {
            print($"[WARNING] No damage on {transform.name} from {other.name}");
        }
    }
}
