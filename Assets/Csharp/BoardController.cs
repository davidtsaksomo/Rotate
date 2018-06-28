using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class BoardController : MonoBehaviour {


	public GameObject[] board;
	public GameObject[] starmount;
	int sum = 0;
	// Use this for initialization
	void Start () {
		if (PlayerPrefs.GetInt ("congratPref", 0) == 1) {
			if (board [0]) {
				board [0].SetActive (true);
				AudioController.success.Play ();
			}
		}
		if (!PlayerPrefs.HasKey ("unlock") && LevelStar.starr > 89 && PlayerPrefs.GetInt("Level45", 0)>0) {
			if (board [1]) {
				board [1].SetActive (true);
				AudioController.success.Play ();
			}
		}
		if (!PlayerPrefs.HasKey ("complete") && LevelStar.starr== 150) {
			if (board [2]) {
				board [2].SetActive (true);
				AudioController.success.Play ();
			}
		}


		for (int i = 1; i < 51; i++) {

			if (PlayerPrefs.HasKey ("Level" + i)) {
				sum = sum + PlayerPrefs.GetInt ("Level" + i);
			}
		}
		if (starmount [0]) {
			starmount [0].GetComponent<Text> ().text = "x " + sum;
		}
		if (starmount [1]) {
			starmount [1].GetComponent<Text> ().text = "x " + sum;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void board0Close(){
		if (board [0]) {
			board [0].SetActive (false);
			AudioController.clickSound.Play ();
			PlayerPrefs.SetInt ("congratPref", 0);
		}
	}
	public void board1Close(){
		if (board [1]) {
			board [1].SetActive (false);
			AudioController.clickSound.Play ();
			PlayerPrefs.SetInt ("unlock", 0);
		}
	}
	public void board2Close(){
		if (board [2]) {
			board [2].SetActive (false);
			AudioController.clickSound.Play ();
			PlayerPrefs.SetInt ("complete", 0);
		}
	}

}
