using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleSystemHandler : MonoBehaviour {

	public ParticleSystem particleSystem;

	// Use this for initialization
	void Start () {
		foreach (var value in World.RocketBlueprint) {
			var handleExplosion = particleSystem.GetComponent<HandleExplosion> ();
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
		particleSystem.Emit (1);

	}
	
	// Update is called once per frame
	void Update () {       
	}
}
