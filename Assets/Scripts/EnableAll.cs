﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableAll : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        for (var i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(true);
    }
}
