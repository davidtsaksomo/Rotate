using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class Status : MonoBehaviour {
	public bool gearOrLantern;
	// Use this for initialization
	void Start () {
		if (gearOrLantern) {
			NotificationCenter.DefaultCenter ().AddObserver (this, "GearStatus");
			GetComponent<Text>().text = ":"+PlayerPrefs.GetInt("MagicGear", 10);
		} else {
			NotificationCenter.DefaultCenter ().AddObserver (this, "LanternStatus");
			GetComponent<Text>().text = ":"+PlayerPrefs.GetInt("MagicLantern", 3);

		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void GearStatus (Notification notification) {
		GetComponent<Text>().text = ":"+PlayerPrefs.GetInt("MagicGear", 10);

	}
	void LanternStatus (Notification notification) {
		GetComponent<Text>().text = ":"+PlayerPrefs.GetInt("MagicLantern", 3);
		print ("dd");
	}
}
