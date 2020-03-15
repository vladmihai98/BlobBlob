using UnityEngine;

public class TrapDoor : MonoBehaviour
{
    [SerializeField] Vector3 positionOffset;
    [SerializeField] CeilingTarget leftTarget;
    [SerializeField] CeilingTarget rightTarget;
    [SerializeField] Transform leverToRevealLeft;
    [SerializeField] Transform leverToRevealRight;

    private Vector3 newPosition;
    private bool revivedMonty;

    void Start()
    {
        newPosition = transform.position + positionOffset;
        revivedMonty = false;
    }

    void Update()
    {
        if(leftTarget.IsSolved() && rightTarget.IsSolved())
        {
            transform.position = newPosition;
            leverToRevealLeft.gameObject.SetActive(true);
            leverToRevealRight.gameObject.SetActive(true);

            // Allow Monty to act again.
            if(!revivedMonty)
            {
                FindObjectOfType<GameController>().FakeMontyNotAlive(true);
                revivedMonty = true;
            }
        }
    }
}
