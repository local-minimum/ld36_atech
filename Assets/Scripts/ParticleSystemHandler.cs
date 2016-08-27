using UnityEngine;
using System.Collections;

public class ParticleSystemHandler : MonoBehaviour {
	public ParticleSystem m_System;

	// Use this for initialization
	void Start () {
		m_System.Emit (1);
	}
	
	// Update is called once per frame
	void Update () {	       
	}
}
