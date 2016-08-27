using UnityEngine;
using System.Collections;

public class EmitSound : MonoBehaviour {

	public AudioSource source;

	void OnParticleCollision(GameObject other) {
		source.Play ();
	}

}
