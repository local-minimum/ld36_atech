using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

struct RocketLayout
{
    public Powder powder;
    public Charge charge;

    public RocketLayout(Powder powder, Charge charge, params RocketComponent[] components)
    {        
        this.powder = powder;
        this.charge = charge;

        for(int i=0; i<components.Length; i++)
        {
            RocketComponent comp = components[i];
            
            if (typeof(Powder) == comp.GetType())
            {
                this.powder = (Powder) comp;
            } else
            {
                this.charge = (Charge) comp;
            }
        }
    }
}


public class ParticleSystemHandler : MonoBehaviour {

	public GameObject prefab;
	public Charge testCharge;
	public Powder testPowder;

	void SendFireworks (GameObject instance, RocketLayout rocket)
	{
		float diff = Random.Range (-5.0f, 5.0f);
		instance.transform.position = new Vector3 (instance.transform.position.x + diff, instance.transform.position.y, instance.transform.position.z);
		var particleSystem = instance.GetComponent<ParticleSystem> ();
		var handleExplosion = particleSystem.GetComponent<HandleExplosion> ();
		handleExplosion.powder = rocket.powder;
		handleExplosion.charge = rocket.charge;
		StartCoroutine (Emit (particleSystem));
	}

	private IEnumerator Emit(ParticleSystem particleSystem) {
		yield return new WaitForSeconds(Random.Range(0.3f, 2f));
		int count = Random.Range (0, 100);
		for (int i = 0; i < count; ++i) {
			particleSystem.SetParticles (new ParticleSystem.Particle[0], 0);
			particleSystem.Emit (1);
		}
	}

	void Start () {

        List<RocketLayout> rockets = GetRockets();
        RocketLayout rocket;
        int nRockets = rockets.Count;
		for (int i = 0; i < 3; ++i) {
            if (i < nRockets)
            {
                rocket = rockets[i];
            } else
            {
                rocket = rockets[Random.Range(0, nRockets)];
            }
			GameObject instance = Instantiate(prefab);
			SendFireworks (instance, rocket);
		}
	}


    List<RocketLayout> GetRockets()
    {
        List<RocketLayout> rockets = new List<RocketLayout>();
        RocketLayout rocket;

        for (int lvl = 0; lvl < 3; lvl++) {
            RocketComponent[] components = World.RocketBlueprint.Values.Where(kvp => kvp.Key == lvl).Select(kvp => kvp.Value).ToArray();
            if (components.Length > 0)
            {
                rocket =  new RocketLayout(testPowder, testCharge, components);
                Debug.Log(string.Format("Constructing rocket stage {2} with {0} and {1} (components {3})",
                    rocket.charge, rocket.powder, lvl, string.Join(", ", components.Select(e => e.name).ToArray())));

                rockets.Add(rocket);                
            }
        }

        if (rockets.Count == 0)
        {
            rocket = new RocketLayout(testPowder, testCharge);
            Debug.Log(string.Format("Constructing default rocket with {0} and {1}", rocket.charge, rocket.powder));
            rockets.Add(rocket);
        }
        return rockets;
    }


}
