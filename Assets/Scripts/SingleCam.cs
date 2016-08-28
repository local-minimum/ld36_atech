using UnityEngine;
using System.Collections;

public class SingleCam : MonoBehaviour {

    static SingleCam _cam;

	void Awake () {
	    if (_cam != null && _cam != this)
        {
            Destroy(this);
        }
        _cam = this;
        DontDestroyOnLoad(this);
	}
}
