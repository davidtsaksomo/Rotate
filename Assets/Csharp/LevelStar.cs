using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelStar : MonoBehaviour {
	public string levelString;
	public int levelInt;
	GameObject button;
	public int levelStar;
	public Sprite[] stars;
	public Color color;
	public static int starr;
	// Use this for initialization
	void Start () {
		levelString = gameObject.name;
		levelInt = int.Parse (levelString);

		button = GameObject.Find ("Level" + levelInt);

		button.SetActive (false);

		if (levelInt == 46 )  {
			starr = 0;
			for (int i = 1; i < 51; i++) {
				if (PlayerPrefs.HasKey ("Level" + i)) {
					starr = starr + PlayerPrefs.GetInt ("Level" + i);

				}


			}
			if (starr > 89&& PlayerPrefs.GetInt ("Level45", 0) > 0) {
				button.SetActive (true);
			}

		} else {
			if (PlayerPrefs.GetInt ("Level" + (levelInt - 1), 0) > 0) {
				button.SetActive (true);
			}
		}


		levelStar = PlayerPrefs.GetInt ("Level" + levelInt, 0);
		if (button.activeSelf == true) {
			GetComponent<Image>().color = color;

			GetComponent<Image>().sprite = stars[levelStar];
		}




	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
