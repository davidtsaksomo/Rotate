using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Lerp : MonoBehaviour {
	public float x;
	public float y;
	public float t;
	Vector3 tujuan;
	public Scrollbar scroll;
	float i = 1f;
	public bool sLerp = false;
//	RectTransform CanvasRect = Canvas.GetComponent<RectTransform>();
	// Use this for initialization
	void Start () {
		x = x * Screen.height / 480;
		y = y * Screen.height / 800;
		tujuan = new Vector3 (transform.position.x - x, transform.position.y - y, 0);

//		tujuan = new Vector3 (((transform.position.x*CanvasRect.sizeDelta.x)-(CanvasRect.sizeDelta.x*0.5f)), ((transform.position.y*CanvasRect.sizeDelta.y)-(CanvasRect.sizeDelta.y*0.5f)), 0);
	}
	
	// Update is called once per frame
	void Update () {
		if (i > 0.99f) {
			if (sLerp) {
				transform.position = (Vector3.Slerp (transform.position, tujuan, t));
			} else {transform.position = (Vector3.Lerp (transform.position, tujuan, t));
			}
		} else if (i > 0.975) {
			transform.position = (Vector3.Lerp (transform.position, tujuan + new Vector3 (x, y, 0), t));
		} else {
			transform.position = (tujuan + new Vector3 (x, y, 0));
		}
	
	
	}
	public void iChange (float iN) {
		i = iN;
		if (PlayerPrefs.GetInt ("Level30", 0) == 0) {
			//i = Mathf.Clamp (i, 0.6f, 1f);
			//scroll.GetComponent<Scrollbar> ().value = i;
		}
	}

}
