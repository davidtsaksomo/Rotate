using UnityEngine;
using System.Collections;

public class Gear : MonoBehaviour {
	float d;
	int range;
	float z;

	public bool pivotGear;
	public GameObject stairschild;
	public bool affectedGear;
	public bool affectedStair;
	GameObject affectedBy;
	float lastPos;
	float deltaPos;
	public int speed;
	// Use this for initialization
	void Start () {
		if (gameObject.name == "GearFront") {
			affectedGear = true;
			speed = -1;
		}
		if (gameObject.name == "GearFront1" || gameObject.name == "GearFront2" || gameObject.name == "GearFront3") {
			affectedStair = true;
		}
		if (affectedGear) {
			if(transform.position.x >0)
				affectedBy = GameObject.Find ("GearFront1");
			if(transform.position.x <0)
				affectedBy = GameObject.Find ("GearFront2");
			lastPos = affectedBy.transform.eulerAngles.z;
		}




		if(stairschild)
		stairschild.GetComponent<Rotate>().isachild = true;

		d = transform.localRotation.z;
	
	}
	
	// Update is called once per frame
	void Update () {
		if (pivotGear && stairschild) {
			stairschild.transform.position = transform.position;
		}
		else if(affectedGear){
	
				deltaPos = affectedBy.transform.eulerAngles.z - lastPos;
				lastPos = affectedBy.transform.eulerAngles.z;
				z += deltaPos;
		
			transform.localRotation = Quaternion.Euler (0,0,z*speed*1.5f);

		}
		else if (!affectedGear && !affectedStair) {
	
			d = d + speed * Time.deltaTime;
			transform.localRotation = Quaternion.Euler (transform.localRotation.x, transform.localRotation.y, d);
		}
	}
}
