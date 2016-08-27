using UnityEngine;
using System.Collections;

public class ParticleSystemHandler : MonoBehaviour {

	public ParticleSystem particleSystem;
	public Charge charge;
	public Powder powder;

	// Use this for initialization
	void Start () {
		particleSystem.Emit (1);
	}
	
	// Update is called once per frame
	void Update () {	       
	}
}
