using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleSystemHandler : MonoBehaviour {

	public ParticleSystem particleSystem;
	public Charge charge;
	public Powder powder;

	// Use this for initialization
	void Start () {
		particleSystem.Emit (1);
		StartCoroutine (EmitMore(2));

	}
	
	// Update is called once per frame
	void Update () {	       
	}
			IEnumerator<WaitForSeconds> EmitMore(int steps)
			{
				for (int i = 0; i <steps; i ++)
				{			
					particleSystem.Emit (1);
					yield return new WaitForSeconds(1);
				}
			}
}
