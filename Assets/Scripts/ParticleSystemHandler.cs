using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleSystemHandler : MonoBehaviour {

	public GameObject prefab;
	public Charge testCharge;
	public Powder testPowder;

	void SendFireworks (GameObject instance)
	{
		float diff = Random.Range (-5.0f, 5.0f);
		instance.transform.position = new Vector3 (instance.transform.position.x + diff, instance.transform.position.y, instance.transform.position.z);
		var particleSystem = instance.GetComponent<ParticleSystem> ();
		var handleExplosion = particleSystem.GetComponent<HandleExplosion> ();
		handleExplosion.powder = testPowder;
		handleExplosion.charge = testCharge;
		foreach (var value in World.RocketBlueprint) {
			RocketComponent component = value.Value.Value;
			Charge charge = component as Charge;
			if (charge != null) {
				handleExplosion.charge = charge;
			}
			Powder powder = component as Powder;
			if (powder != null) {
				handleExplosion.powder = powder;
			}
		}
		StartCoroutine (Emit (particleSystem));
	}

	private IEnumerator Emit(ParticleSystem particleSystem) {
		yield return new WaitForSeconds(Random.Range(0.3f, 2f));
		int count = (int)Random.Range (0, 100);
		for (int i = 0; i < count; ++i) {
			particleSystem.SetParticles (new ParticleSystem.Particle[0], 0);
			particleSystem.Emit (1);
		}
	}

	// Use this for initialization
	void Start () {
		for (int i = 0; i < 3; ++i) {
			GameObject instance = Instantiate(prefab);
			SendFireworks (instance);
		}
	}
	
	// Update is called once per frame
	void Update () {       
	}
}
