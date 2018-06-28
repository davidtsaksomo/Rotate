using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class LevelButton : MonoBehaviour {
	public int level;
	int stars;

	// Use this for initialization
	void Awake () {			

		if (!PlayerPrefs.HasKey ("Level0"))
			PlayerPrefs.SetInt ("Level0", 3);

	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
