using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class InfiniteFireworks : MonoBehaviour {

    public AudioMixerSnapshot mixerSnapshot;
	public GameObject prefab;
	public Charge[] charges;
	public Powder[] powders;

	void SendFireworks (GameObject instance, Charge charge, Powder powder)
	{
		float diff = Random.Range (-5.0f, 5.0f);
		instance.transform.position = new Vector3 (instance.transform.position.x + diff, instance.transform.position.y, instance.transform.position.z);
		var particleSystem = instance.GetComponent<ParticleSystem> ();
		var handleExplosion = particleSystem.GetComponent<HandleExplosion> ();
		handleExplosion.disableNextScene = true;
		handleExplosion.charge = charge;
		handleExplosion.powder = powder;
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

	private IEnumerator CreateInfiniteFireworks() {
		for (;;) {
			GameObject instance = Instantiate(prefab);
			int chargeIndex = (int)Random.Range (0f, (float)charges.Length);
			int powderIndex = (int)Random.Range (0f, (float)powders.Length);
			SendFireworks (instance, charges[chargeIndex], powders[powderIndex]);
			yield return new WaitForSeconds(Random.Range(0.3f, 1.5f));
		}
	}

	// Use this for initialization
	void Start () {
        mixerSnapshot.TransitionTo(0.5f);
		StartCoroutine (CreateInfiniteFireworks ());
	}

	// Update is called once per frame
	void Update () {       
	}
}
