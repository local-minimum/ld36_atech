using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Customer : MonoBehaviour {

    Canvas canvas;

    void Start () {
        canvas = GetComponent<Canvas>();
	}
	
    public void HideCustomer()
    {
        canvas.enabled = false;
    }
}
