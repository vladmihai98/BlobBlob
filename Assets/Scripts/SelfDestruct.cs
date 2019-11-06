﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    [SerializeField] float secondsAlive = 1f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, secondsAlive);
    }
}
