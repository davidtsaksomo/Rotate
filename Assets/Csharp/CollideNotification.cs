using UnityEngine;
using System.Collections;

public class CollideNotification : MonoBehaviour {
	public GameObject NotificationManager;
	public GameObject Correct;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnTriggerEnter2D(Collider2D other)
	{			print (this.gameObject.name + "intersect" + other.gameObject.name);


		if (other.gameObject.name == Correct.name){
			NotificationManager.GetComponent<CompleteChecker>().amount++;
		}
	}
	void OnTriggerExit2D(Collider2D other)
	{if (other.gameObject.name == Correct.name){			
			NotificationManager.GetComponent<CompleteChecker>().amount--;
			}
	}
}
