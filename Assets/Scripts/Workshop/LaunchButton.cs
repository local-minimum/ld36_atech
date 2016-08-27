using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LaunchButton : MonoBehaviour {

    public string launchScene;

    public void Launch()
    {
        Customer.customerMode = CustomerMode.Pay;
        SceneManager.LoadScene(launchScene);
        
    }
}
