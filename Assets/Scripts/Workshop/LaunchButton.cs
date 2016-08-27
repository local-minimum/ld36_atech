﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LaunchButton : MonoBehaviour {

    public string launchScene;
    Workshop workshop;
    Button btn;

    void Awake()
    {
        workshop = GetComponentInParent<Workshop>();
        btn = GetComponent<Button>();
    }

    void OnEnable()
    {
        workshop.OnRocketAction += Workshop_OnRocketAction;
    }

    void OnDisable()
    {
        workshop.OnRocketAction -= Workshop_OnRocketAction;
    }

    private void Workshop_OnRocketAction(RocketEventsTypes type)
    {
        btn.interactable = type == RocketEventsTypes.Ready;
    }

    public void Launch()
    {
        Customer.customerMode = CustomerMode.Pay;
        SceneManager.LoadScene(launchScene);
        
    }

}
