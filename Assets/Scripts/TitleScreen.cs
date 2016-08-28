﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour {

	public void StartGame()
	{
		SceneManager.LoadScene("customer_workshop");
	}

	public void ExitGame() {
		Application.Quit ();
	}
}
