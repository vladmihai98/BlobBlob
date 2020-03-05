using UnityEngine;

public class CeilingCrate : MonoBehaviour
{
    [SerializeField] Vector3 positionOffset;

    private Vector3 initPosition;
    private Vector3 newPosition;
    private bool isGoingTowardsInitPosition;
    private bool isGoingAwayFromInitPosition;

    void Start()
    {
        initPosition = transform.position;
        newPosition = transform.position + positionOffset;
        isGoingAwayFromInitPosition = true;
        isGoingTowardsInitPosition = false;
    }

    void Update()
    {
        if(gameObject.activeSelf)
        {
            MoveBox();
        }
    }

    void MoveBox()
    {
        if(isGoingAwayFromInitPosition)
        {
            transform.position = Vector3.Lerp(transform.position, newPosition, 0.15f);
            if(transform.position == newPosition)
            {
                isGoingAwayFromInitPosition = false;
                isGoingTowardsInitPosition = true;
            }
        }

        if(isGoingTowardsInitPosition)
        {
            transform.position = Vector3.Lerp(transform.position, initPosition, 0.15f);
            if(transform.position == initPosition)
            {
                isGoingAwayFromInitPosition = true;
                isGoingTowardsInitPosition = false;
            }
        }
    }
}
