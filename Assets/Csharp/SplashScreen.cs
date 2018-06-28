using UnityEngine;
using System.Collections;

public class SplashScreen : MonoBehaviour {

	// Use this for initialization
	void Start () {
		AudioController.splash.Play ();
		Invoke ("load", 1f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void load(){
		Application.LoadLevelAsync ("MainMenu");
	}
}
