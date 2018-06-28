using UnityEngine;
using System.Collections;

public class Orb : MonoBehaviour {
	Vector3 dest;
	// Use this for initialization
	void Start () {
		dest = transform.position;

	}
	
	// Update is called once per frame
	void Update () {
		transform.position = Vector3.Lerp (transform.position,dest , 3.3f*Time.deltaTime);
	}
	void OnTriggerEnter2D(Collider2D other){
		if (other.gameObject.tag == "Player") {
			dest = new Vector3 (-3f, 5f,0);	
			AudioController.pickup.Play ();
			Destroy(gameObject, 2f);
		}
	}
}
