using UnityEngine;
using System.Collections;

public class HandDestroyer : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Mathf.Round (transform.position.x * 1000) / 1000 == 0.583) {
			Destroy(GameObject.Find ("Hand"));
		}
	}
}
