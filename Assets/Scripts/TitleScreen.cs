using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class TitleScreen : MonoBehaviour {

	public AudioMixerSnapshot audioMixerSnapshot;

    void Start() {
		audioMixerSnapshot.TransitionTo (0.5f);
	}

	public void StartGame()
	{
		World.Reset ();
		SceneManager.LoadScene("customer_workshop");
	}

	public void ExitGame() {
		Application.Quit ();
	}
}
