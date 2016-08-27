using UnityEngine;
using System.Collections;

public class HandleExplosion : MonoBehaviour {

	public ParticleSystem subSystem;
	public GameObject mesh;
	public Charge charge;
	public Powder powder;
	ParticleSystem.Particle[] particles = new ParticleSystem.Particle[10];

	void OnParticleCollision(GameObject other) {
		var system = this.GetComponent<ParticleSystem> ();
		int num = system.GetParticles (particles);
		if (num > 0) {
			var filter = mesh.GetComponent<MeshFilter> ();
			filter.mesh = charge.explosionShape;
			mesh.transform.localPosition = particles[0].position;
			var emitter = mesh.GetComponent<MeshParticleEmitter> ();
			system.SetParticles (new ParticleSystem.Particle[0], 0);
			var renderer = mesh.GetComponent<ParticleRenderer> ();
			renderer.material.SetColor ("_TintColor", Color.red);
			subSystem.transform.localPosition = particles [0].position;	
			subSystem.Emit (charge.explosionShape.vertexCount);
			var particles2 = new ParticleSystem.Particle[charge.explosionShape.vertexCount];
			subSystem.GetParticles (particles2);
			for (var i = 0; i < charge.explosionShape.vertexCount; ++i) {
				var vertex = charge.explosionShape.vertices [i];
				var val = Random.Range (0.9f, 1.1f);
				float range = 0.1f;
				var valx = Random.Range (-range, range);
				var valy = Random.Range (-range, range);
				var valz = Random.Range (-range, range);
				particles2 [i].position = vertex * 10 + new Vector3(valx, valy, valz);
				particles2 [i].startSize = 0.1f;
				particles2 [i].lifetime = 2 * val;
				particles2 [i].velocity = (vertex * 100) * val;
			}
			subSystem.transform.Rotate (new Vector3 (90, 0, 0));
			subSystem.SetParticles (particles2, particles2.Length);
	//		emitter.Emit (100);
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
