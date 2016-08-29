using UnityEngine;
using System.Collections.Generic;

public class ShootingSoundController : MonoBehaviour {

    [SerializeField]
    AudioClip shootSound;

    [SerializeField]
    AudioClip[] explodeSounds;

    [SerializeField]
    AudioSource speaker;

    public void Shoot()
    {
        speaker.PlayOneShot(shootSound);
    }

    public void Explode()
    {
        speaker.PlayOneShot(explodeSounds[Random.Range(0, explodeSounds.Length)]);
    }
    
}
