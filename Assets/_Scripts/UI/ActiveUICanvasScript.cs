﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveUICanvasScript : MonoBehaviour {

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

}
