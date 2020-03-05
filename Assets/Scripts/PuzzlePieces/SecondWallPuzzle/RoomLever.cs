using System.Collections;
using UnityEngine;

public class RoomLever : MonoBehaviour
{
    private bool leverPushed;
    private Quaternion initialLocalRotation;
    private Quaternion newRotation;

    void Start()
    {
        initialLocalRotation = transform.localRotation;
        newRotation = Quaternion.Euler(initialLocalRotation.eulerAngles.x, initialLocalRotation.eulerAngles.y, 45f);
    }

    void Update()
    {
        if(leverPushed)
        {
            RotateLever();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(!leverPushed)
        {
            leverPushed = true;
        }
    }

    void RotateLever()
    {
        transform.localRotation = Quaternion.Lerp(transform.localRotation, newRotation, 0.1f);

        if (transform.localRotation == newRotation)
        {
            StartCoroutine(ResetLever());
        }
    }

    IEnumerator ResetLever()
    {
        yield return new WaitForSecondsRealtime(1.5f);

        transform.localRotation = initialLocalRotation;
        leverPushed = false;
    }

    public bool IsLeverPushed()
    {
        return leverPushed;
    }
}
