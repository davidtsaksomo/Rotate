using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class Sliders : MonoBehaviour {
	public bool MusicOrSfx;
	Slider volumeSlider;
	// Use this for initialization
	void Start () {
		if (MusicOrSfx) {
			volumeSlider = GetComponent<Slider> ();
			volumeSlider.value = PlayerPrefs.GetFloat ("musicVolume", 1);
		} else {
			volumeSlider = GetComponent<Slider> ();
			volumeSlider.value = PlayerPrefs.GetFloat ("sfxVolume", 1);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
