﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class HandleExplosion : MonoBehaviour {

	public ParticleSystem subSystem;
	public Charge charge;
	public Powder powder;
	bool started = false;
	public bool activateSceneTransition = false;
	ParticleSystem.Particle[] particles = new ParticleSystem.Particle[10];
	public AudioSource source;    

    [SerializeField]
    int nSoundsMin = 5;

    [SerializeField]
    int nSoundsMax = 10;

    [SerializeField]
    float dSoundMin = 0.1f;

    [SerializeField]
    float dSoundMax = 0.5f;

	private IEnumerator PlaySounds(AudioClip clip) {
        yield return new WaitForSeconds(0.4f);

        for (int i=0, l=Random.Range(nSoundsMin, nSoundsMax); i<l; i++)
        {
            source.PlayOneShot(clip, 0.1f);
            yield return new WaitForSeconds(Random.Range(dSoundMin, dSoundMax));
        }
	}

    IEnumerator WaitForDeath()
    {
        yield return new WaitForSeconds(3.5f);
        if (activateSceneTransition)
        {
            SceneManager.LoadScene("customer_workshop");
        }
        else {
            Destroy(gameObject);
        }
    }

	void OnParticleCollision(GameObject other) {
		source.clip = powder.audio;
		if (source.clip != null) {
			StartCoroutine (PlaySounds (source.clip));
		}

        StartCoroutine(WaitForDeath());

		var system = this.GetComponent<ParticleSystem> ();
		int num = system.GetParticles (particles);
		if (num > 0) {
			started = true;
			system.SetParticles (new ParticleSystem.Particle[0], 0);
			subSystem.transform.localPosition = particles [0].position;
			var renderer = subSystem.GetComponent<Renderer> ();
			renderer.material = powder.particleMaterial;

			var particleCount = (int)(100 * charge.shapeSize);	
			subSystem.Emit (particleCount);
			var particles2 = new ParticleSystem.Particle[particleCount];
			subSystem.GetParticles (particles2);

			var col = subSystem.colorOverLifetime;
			col.enabled = true;
			Gradient grad = new Gradient();
			grad.SetKeys( new GradientColorKey[] {
				new GradientColorKey(powder.color, 0.0f), 
				new GradientColorKey(powder.color, 1.0f) },
				new GradientAlphaKey[] { 
					new GradientAlphaKey(1.0f, 0.0f), 
					new GradientAlphaKey(1.0f, 1.0f) } );
			col.color = grad;

			for (var i = 0; i < particleCount; ++i) {
				var idx = Random.Range (0, charge.explosionShape.vertexCount);
				var vertex = charge.explosionShape.vertices [(int)idx];
				var val = Random.Range (0.9f, 1.1f);
				float range = 0.2f;
				var valx = Random.Range (-range, range);
				var valy = Random.Range (-range, range);
				var valz = Random.Range (-range, range);
				particles2 [i].position = vertex * 10 + new Vector3(valx, valy, valz);
				float startSize = Random.Range(0.4f, 1f);
				particles2 [i].startSize = startSize;
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

}
