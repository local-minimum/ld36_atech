using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Audio;

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
	public Charge defaultCharge;
	public Powder defaultPowder;
    public string[] componentLookup;
    public RocketComponent[] rocketComponents;
    public AudioMixerSnapshot mixerSnapshot;
    public int numberOfFireworksMin = 6;
    public int numberOfFireworksMax = 30;
    public float launchDelayMin = 0f;
    public float launchDelayMax = 1f;
    int toLaunch = -1;

	void SendFireworks (GameObject instance, RocketLayout rocket, int launchSequenceIndex)
	{
		float diff = Random.Range (-5.0f, 5.0f);
		instance.transform.position = new Vector3 (instance.transform.position.x + diff, instance.transform.position.y, instance.transform.position.z);
		var particleSystem = instance.GetComponent<ParticleSystem> ();
		var handleExplosion = particleSystem.GetComponent<HandleExplosion> ();
		handleExplosion.powder = rocket.powder;
		handleExplosion.charge = rocket.charge;
		StartCoroutine (Emit (particleSystem, launchSequenceIndex));
	}

	private IEnumerator Emit(ParticleSystem particleSystem, int launchSequenceIndex) {

        ShootingSoundController sCtrl = particleSystem.GetComponentInChildren<ShootingSoundController>();

        while (toLaunch < launchSequenceIndex)
        {
            yield return new WaitForSeconds(0.01f);
        }

		yield return new WaitForSeconds(Random.Range(launchDelayMin, launchDelayMax));
        toLaunch = launchSequenceIndex + 1;

        sCtrl.Shoot();
		int count = Random.Range (50, 100);
		for (int i = 0; i < count; ++i) {
			particleSystem.SetParticles (new ParticleSystem.Particle[0], 0);
			particleSystem.Emit (1);
		}
        yield return new WaitForSeconds(1.5f);
        sCtrl.Explode();
	}

	void Start () {

        mixerSnapshot.TransitionTo(0.5f);


        //Test that all exists...
        foreach (string identifier in Workshop.ingredients.Keys)
        {
            GetRocketComponent(identifier);
        }

        List<RocketLayout> rockets = GetRockets();
        RocketLayout rocket;
        int nRockets = rockets.Count;
        int numberOfFireworks = Random.Range(numberOfFireworksMin, numberOfFireworksMax);
        toLaunch = 0;
		for (int i = 0; i < numberOfFireworks; ++i) {
            if (i < nRockets)
            {
                rocket = rockets[i];
            } else
            {
                rocket = rockets[Random.Range(0, nRockets)];
            }
			GameObject instance = Instantiate(prefab);
			SendFireworks (instance, rocket, i);
		}
	}


    List<RocketLayout> GetRockets()
    {
        List<RocketLayout> rockets = new List<RocketLayout>();
        RocketLayout rocket;

        for (int lvl = 0; lvl < 3; lvl++) {
            string[] identifiers = World.RocketBlueprint.Values.Where(kvp => kvp.Key == lvl).Select(kvp => kvp.Value).ToArray();
            if (identifiers.Length > 0)
            {

                RocketComponent[] componentArr = GetRocketComponents(identifiers).ToArray();

                rocket =  new RocketLayout(defaultPowder, defaultCharge, componentArr);
                Debug.Log(string.Format("Constructing rocket stage {2} with {0} and {1} (components {3})",
                    rocket.charge, rocket.powder, lvl, string.Join(", ", componentArr.Select(e => e.name).ToArray())));

                rockets.Add(rocket);                
            }
        }

        if (rockets.Count == 0)
        {
            rocket = new RocketLayout(defaultPowder, defaultCharge);
            Debug.Log(string.Format("Constructing default rocket with {0} and {1}", rocket.charge, rocket.powder));
            rockets.Add(rocket);
        }
        return rockets;
    }

    List<RocketComponent> GetRocketComponents(params string[] identifiers)
    {
        List<RocketComponent> ret = new List<RocketComponent>();
        for (int i=0; i< identifiers.Length; i++)
        {
            ret.Add(GetRocketComponent(identifiers[i]));
        }
        return ret;
    }

    RocketComponent GetRocketComponent(string identifier)
    {
        for (int i=0; i<componentLookup.Length; i++)
        {
            if (componentLookup[i] == identifier)
            {
                return rocketComponents[i];
            }
        }

        Debug.LogError(string.Format("Could not loook-up {0}", identifier));
        return null;
    }


}
