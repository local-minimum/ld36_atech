using UnityEngine;
using System.Collections;

public class Paralax : MonoBehaviour {

	[SerializeField] Camera cam;
	[SerializeField] float factor = 1;
	Vector3 origin;
	float depth;

	void Start () {
		origin = transform.position;
		depth = transform.position.z;
	}
	
	Vector2 angles {
		get {
			Vector3 delta = origin - cam.transform.position;
			delta /= delta.magnitude;
			return new Vector2 (Mathf.Acos(delta.x), Mathf.Acos(delta.y));
		}
	}

	void Update () {
	
	}
}
