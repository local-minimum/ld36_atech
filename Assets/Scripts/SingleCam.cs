using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SingleCam : MonoBehaviour {

    static SingleCam _cam;

    [SerializeField] AudioSource customerOrder;

    [SerializeField] AudioSource customerResponse;

    [SerializeField] AudioSource buttons;

    Camera _theCam;

    void Awake () {
        Input.simulateMouseWithTouches = true;
        if (_cam != null && _cam != this)
        {
            Destroy(gameObject);
        }
        else {
            _cam = this;
            _theCam = GetComponent<Camera>();
            DontDestroyOnLoad(this);
        }
	}

	void Update() {
		if (Input.GetKey (KeyCode.Escape)) {
			if (SceneManager.GetActiveScene ().name == "title") {
				Application.Quit ();
			} else {
				SceneManager.LoadScene ("title");
			}
		}
	}

    public static AudioSource CustomerOrderSpeaker
    {
        get
        {
            return _cam.customerOrder;
        }
    }

    public static AudioSource CustomerResponseSpeaker
    {
        get
        {
            return _cam.customerResponse;
        }
    }

    public static AudioSource ButtonSpeaker
    {
        get
        {
            return _cam.buttons;
        }
    }

    public static Camera Cam
    {
        get
        {
            return _cam._theCam;
        }
    }
}
