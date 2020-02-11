using UnityEngine;

public class MontyController : Character
{
    //TODO remove multiple jump; isGrounded?

    [Header("Extra Stats")]
    [SerializeField] float jumpHeight = 300;

    private Rigidbody rigidbody;
    private Vector3 velocity;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        currentHealth = MaxHealth;
    }

    public override void Interact()
    {
        ProcessInput();
        Move();
        Jump();
    }

    /// <summary>
    /// Process the keyboard input to decide move direction.
    /// </summary>
    void ProcessInput()
    {
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
        {
            velocity = Vector3.forward + Vector3.left;
        }
        else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
        {
            velocity = Vector3.forward + Vector3.right;
        }
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
        {
            velocity = Vector3.back + Vector3.left;
        }
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
        {
            velocity = Vector3.back + Vector3.right;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            velocity = Vector3.forward;
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            velocity = Vector3.zero;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            velocity = Vector3.left;
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            velocity = Vector3.zero;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            velocity = Vector3.back;
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            velocity = Vector3.zero;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            velocity = Vector3.right;
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            velocity = Vector3.zero;
        }
    }

    void Move()
    {
        rigidbody.MovePosition(transform.position + (velocity * MovementSpeed * Time.deltaTime));
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rigidbody.AddForce(0, jumpHeight, 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Ability ability = other.GetComponent<Ability>();

        if (ability.AttackDamage > 0)
        {
            TakeDamage(ability.AttackDamage, Resistance.UseArmor);
        }
        else if (ability.AbilityPower > 0)
        {
            TakeDamage(ability.AbilityPower, Resistance.UseMagicResist);
        }
        else
        {
            print($"[WARNING] No damage on {transform.name} from {other.name}");
        }
    }
}
