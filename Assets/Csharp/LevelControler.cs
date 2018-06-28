using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelControler : MonoBehaviour {

	public GameObject loadingScreen;
	public GameObject levelSelect;
	public GameObject parent;
	public Sprite levelselect;

	void Awake(){
		if(PlayerPrefs.GetInt("Level30", 0) > 0){
			//levelSelect.GetComponent<Image> ().sprite = levelselect;
			//levelSelect.GetComponent<RectTransform> ().localScale = new Vector2 (362.46f, 4000f);
			//levelSelect.GetComponent<RectTransform> ().localPosition = new Vector2(-10f,-4000f);
			levelSelect.SetActive(true);
			parent.GetComponent<ScrollRect> ().content = levelSelect.GetComponent<RectTransform>();
		}
		if(PlayerPrefs.GetInt("Level45", 0) > 0){
			//levelSelect.GetComponent<Image> ().sprite = levelselect;
			//levelSelect.GetComponent<RectTransform> ().localScale = new Vector2 (362.46f, 4000f);
			//levelSelect.GetComponent<RectTransform> ().localPosition = new Vector2(-10f,-4000f);
			congratPref();
		}
	}
	// Use this for initialization
	void Start () {
		AudioController.backgroundMusic.Stop ();
		AudioController.ossuaryRest.Stop ();
		AudioController.chainDrag.Stop ();
		AudioController.GonnaStart.Play ();
		AudioController.carefree.Stop ();
		AudioController.stairHum.Stop ();
		AudioController.lifeOfRiley.Stop ();
		AudioController.liftMoto.Stop ();

		//////////////TEST MODE
		if(!PlayerPrefs.HasKey("MagicLantern")){
			PlayerPrefs.SetInt ("MagicLantern", 3);
		}
		if(!PlayerPrefs.HasKey("MagicGear")){
			PlayerPrefs.SetInt ("MagicGear", 999);
		}
		//PlayerPrefs.SetInt ("MagicLantern", 999);
		//PlayerPrefs.SetInt ("MagicGear", 999);
		if(PlayerPrefs.GetInt("Level30", 0) > 0){
			//levelSelect.GetComponent<Image> ().sprite = levelselect;
			//levelSelect.GetComponent<RectTransform> ().localScale = new Vector2 (362.46f, 4000f);
			//levelSelect.GetComponent<RectTransform> ().localPosition = new Vector2(-10f,-4000f);
			//levelSelect.SetActive(true);
			//GetComponent<ScrollRect> ().content = levelSelect.GetComponent<RectTransform>();
		}


	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void LoadLevel(int level){
		AudioController.clickSound.Play ();
		displayLoadingScreen();
		Application.LoadLevelAsync ("level"+level);
		
	}

	void displayLoadingScreen()
	{if (loadingScreen) {loadingScreen.SetActive(true);
		}
		
	}
	void congratPref(){
		print("dddsss");
		if (!PlayerPrefs.HasKey ("congratPref")) {
			PlayerPrefs.SetInt ("congratPref", 1);
		}
	}
}

