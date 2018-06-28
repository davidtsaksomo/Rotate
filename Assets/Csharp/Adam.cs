using UnityEngine;
using System.Collections;

public class Adam : MonoBehaviour {
	public GameObject adam;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		adam.transform.position = new Vector2 (adam.transform.position.x, adam.transform.position.y);
	}
}
