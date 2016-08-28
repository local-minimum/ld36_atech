using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SingleCam : MonoBehaviour {

    static SingleCam _cam;

	void Awake () {
        if (_cam != null && _cam != this)
        {
            Destroy(gameObject);
        }
        else {
            _cam = this;
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
}
