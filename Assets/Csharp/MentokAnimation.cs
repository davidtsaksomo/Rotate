using UnityEngine;
using System.Collections;

public class MentokAnimation : MonoBehaviour {

	float yscale;
	// Use this for initialization
	void Start () {
		yscale = transform.localScale.y;
	}
	
	// Update is called once per frame
	void Update () {

			transform.localScale = new Vector2 (transform.localScale.x, Mathf.Clamp (transform.localScale.y - 4f * Time.deltaTime, 0f, 2f));

	}
}
