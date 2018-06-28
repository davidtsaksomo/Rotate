using UnityEngine;
using System.Collections;

public class SkipPath : MonoBehaviour {
	public float Degree;
	public Component[] hopath;
	public GameObject gearStair;
	public int num;
	// Use this for initialization
	void Start () {
		InvokeRepeating ("check", 0f, 0.3f);
	}
	
	// Update is called once per frame
	void check () {
		if (gearStair.GetComponent<RotateGear> ().degree == Degree && PlayerPrefs.GetInt("Complete")== 0) {

			for(int i = 0; i < num; i++){
				Destroy(hopath[i]);
			}
			Destroy (this, 0.1f);
		} 
	}
}
