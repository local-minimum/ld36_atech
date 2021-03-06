﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public abstract class RocketComponent : MonoBehaviour {

    public AudioClip audio;
    public string identifier;

    void Reset()
    {
        StoreItem item = GetComponentInParent<StoreItem>();
        if (item)
        {
            item.Blueprint = this;        
        }
    }
}
