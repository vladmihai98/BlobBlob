using System.Collections;
using UnityEngine;

public class RoomGate : MonoBehaviour
{
    [SerializeField] RoomLever leftLever;
    [SerializeField] RoomLever rightLever;
    [SerializeField] Vector3 movementOffset;

    private bool areLeversPushed;
    private bool wallMoved;
    private Vector3 newPosition;

    void Start()
    {
        areLeversPushed = false;
        wallMoved = false;
        newPosition = transform.position + movementOffset;
    }

    void Update()
    {
        if(!areLeversPushed)
        {
            StartCoroutine(CheckRoomLevers());
        }

        if(!wallMoved && areLeversPushed)
        {
            MoveWall();
        }
    }

    IEnumerator CheckRoomLevers()
    {
        if(leftLever.IsLeverPushed())
        {
            if(rightLever.IsLeverPushed())
            {
                areLeversPushed = true;
            }

            yield return new WaitForSecondsRealtime(1f);

            if(rightLever.IsLeverPushed())
            {
                areLeversPushed = true;
            }
        }

        if (rightLever.IsLeverPushed())
        {
            if (leftLever.IsLeverPushed())
            {
                areLeversPushed = true;
            }

            yield return new WaitForSecondsRealtime(1f);

            if (leftLever.IsLeverPushed())
            {
                areLeversPushed = true;
            }
        }
    }

    void MoveWall()
    {
        transform.position = Vector3.Lerp(transform.position, newPosition, 0.05f);

        if(transform.position == newPosition)
        {
            gameObject.SetActive(false);
        }
    }
}
