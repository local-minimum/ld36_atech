using UnityEngine;
using System.Collections;

public class Paralax : MonoBehaviour {

	[SerializeField] Camera cam;
	[SerializeField] float factor = 30;
	Vector3 origin;
	float depth;

	void Start () {
		origin = transform.position;
		depth = transform.position.z - cam.transform.position.z;
	}
	
	Vector2 angles {
		get {
			Vector3 delta = origin - cam.transform.position;			
			return new Vector2 (Mathf.Atan2(delta.x, delta.z), Mathf.Atan2(delta.y, delta.z));
		}
	}

	void Update () {
        Vector2 a = angles;
        
        transform.position = new Vector3(Mathf.Tan(a.x) / depth * factor, Mathf.Tan(a.y) / depth * factor, origin.z); 
	}
}
