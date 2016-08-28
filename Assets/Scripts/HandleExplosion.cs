using UnityEngine;
using System.Collections;

public class HandleExplosion : MonoBehaviour {

	public ParticleSystem subSystem;
	public Charge charge;
	public Powder powder;
	ParticleSystem.Particle[] particles = new ParticleSystem.Particle[10];

	void OnParticleCollision(GameObject other) {
		var system = this.GetComponent<ParticleSystem> ();
		int num = system.GetParticles (particles);
		if (num > 0) {
			system.SetParticles (new ParticleSystem.Particle[0], 0);
			subSystem.transform.localPosition = particles [0].position;	
			var particleCount = (int)(100 * charge.shapeSize);	
			subSystem.Emit (particleCount);
			var particles2 = new ParticleSystem.Particle[particleCount];
			subSystem.GetParticles (particles2);
			for (var i = 0; i < particleCount; ++i) {
				var idx = Random.Range (0, charge.explosionShape.vertexCount);
				var vertex = charge.explosionShape.vertices [(int)idx];
				var val = Random.Range (0.9f, 1.1f);
				float range = 0.2f;
				var valx = Random.Range (-range, range);
				var valy = Random.Range (-range, range);
				var valz = Random.Range (-range, range);
				particles2 [i].position = vertex * 10 + new Vector3(valx, valy, valz);
			//	particles2 [i].color = powder.startColor;
			//	particles2 [i].color = new Color (powder.startColor.r, powder.startColor.g, powder.startColor.b, 1);
				particles2 [i].startSize = 0.2f;
				particles2 [i].lifetime = 2 * val;
				particles2 [i].velocity = (vertex * 100) * val * (float)charge.shapeSize;
			}
			var rotateRange = 20.0f;
			var rotateX = Random.Range (-rotateRange, rotateRange);
			var rotateY = Random.Range (-rotateRange, rotateRange);
			var rotateZ = Random.Range (-rotateRange, rotateRange);
			subSystem.transform.Rotate (new Vector3 (90 + rotateX, 0 + rotateY, 0 + rotateZ));
			subSystem.SetParticles (particles2, particles2.Length);
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
