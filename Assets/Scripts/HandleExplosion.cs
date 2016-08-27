using UnityEngine;
using System.Collections;

public class HandleExplosion : MonoBehaviour {

	public GameObject mesh;
	ParticleSystem.Particle[] particles = new ParticleSystem.Particle[10];

	void OnParticleCollision(GameObject other) {
		var system = this.GetComponent<ParticleSystem> ();
		int num = system.GetParticles (particles);
		if (num > 0) {
			mesh.transform.localPosition = particles[0].position;
			var emitter = mesh.GetComponent<MeshParticleEmitter> ();
			system.SetParticles (new ParticleSystem.Particle[0], 0);
			var renderer = mesh.GetComponent<ParticleRenderer> ();
			renderer.material.SetColor ("_TintColor", Color.red);
			emitter.Emit (100);
		}
	}
	void Update () {	       
		var system = this.GetComponent<ParticleSystem> ();
		int num = system.GetParticles (particles);
		if (num > 0) {
		//	mesh.transform.localPosition = particles[0].position;
		}
	}
}
