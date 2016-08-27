using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public abstract class RocketComponent : MonoBehaviour {

    public string[] judgementProperties;
    public Sprite icon;    
    public string description;
    public AudioClip audio;

    void Reset()
    {
        StoreItem item = GetComponent<StoreItem>();
        if (item)
        {
            item.Blueprint = this;        
        }
    }
}
