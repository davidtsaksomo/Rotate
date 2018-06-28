using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class Stuck : MonoBehaviour {
	public Color alphaChange;
	public bool isAText;
	public float t = 60f;

	// Use this for initialization
	void Start () {
		alphaChange.a = 0f;
		Invoke ("display", t);
		Invoke ("disappear", t+15f);
		Destroy (gameObject, t+20f);
	}
	
	// Update is called once per frame
	void Update(){
		if (isAText)
			GetComponent<Text> ().color = alphaChange;
		else
		GetComponent<Image> ().color = alphaChange;
	}

	void display(){
		StartCoroutine ("fadein");
	}
	void disappear (){
		StartCoroutine ("fadeout");
	}
	IEnumerator fadein(){
		for(float i = 0; i <= 1f; i +=0.02f){
			alphaChange.a = i;
			yield return null;
		}
	}
	IEnumerator fadeout(){
		for(float i = 1f; i >=0; i -=0.01f){
			alphaChange.a = i;
			yield return null;
		}
	}
}
