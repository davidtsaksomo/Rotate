using UnityEngine;
using System.Collections;

public class Movements : MonoBehaviour {
	Vector3 lastposition;
	Vector3 deltaPosition;
	public GameObject CompleteScreen;
	public int i;
	public GameObject[] stars;
	public int levelsekarang;
	public GameObject recordText;
	bool isComplete = false;
	public bool showAd;
	public GameObject skipButt;
	public GameObject lightLantern;
	GameObject overlay;
	int mods;
	// Use this for initialization
	void Awake () {
		lastposition = transform.position;
	}

	void Start(){
		
		showAd = false;

	//	if (Application.loadedLevel > 12 && (Application.loadedLevel % 2==0)) {
	//		showAd = true;
	//	}
		overlay = GameObject.Find ("Overlay");

		if (overlay) {
			overlay.GetComponent<RectTransform> ().anchoredPosition = transform.position * 60;
			overlay.GetComponent<Animator> ().SetBool ("overlay", true);
		}

	    if (!PlayerPrefs.HasKey ("MagicGear"))
			PlayerPrefs.SetInt ("MagicGear", 10);

		if (showAd) {
			GetComponent<ChartBoostCall>().ChaceInterStitial();
		}
		levelsekarang = GameObject.Find ("NotificationManager").GetComponent<CompleteChecker> ().levelSekarang;
	}
	// Update is called once per frame
	void Update () {
		if (PlayerPrefs.GetInt ("Complete", 1) == 0) {
			if(overlay)
			overlay.GetComponent<RectTransform>().anchoredPosition = transform.position*60;
			lightLantern.transform.position = transform.position;
			deltaPosition = transform.position - lastposition;
			if (deltaPosition.x > 0.005) {
				transform.rotation = Quaternion.Euler (0, 0, 0);
			}
			if (deltaPosition.x < -0.005) {
				transform.rotation = Quaternion.Euler (0, 180, 0);
			}
			lastposition = transform.position;
		}
	}
	void OnTriggerEnter2D(Collider2D other){
		if (other.gameObject.name == "Finish" ) {
			//complete
			if(!isComplete)
			completeCall();


		}
		if (other.gameObject.name == "DoorFront") {
			//complete
			if(!isComplete)
				Invoke("completeCall",1f);
			Destroy(GetComponent<SpriteRenderer>(),0.2f);
			
		}
		if (other.gameObject.tag == "Walking") {
			GetComponent<Animator>().SetBool("isWalking", true);
		}
		if (other.gameObject.tag == "Downwards") {
			GetComponent<Animator>().SetBool("isOnBack", false);
			GetComponent<SpriteRenderer>().sortingLayerName = "player";
			lightLantern.GetComponent<SpriteRenderer>().sortingLayerName = "player";


		}
		if (other.gameObject.tag == "Upwards") {
			GetComponent<Animator>().SetBool("isOnBack", true);
			GetComponent<SpriteRenderer>().sortingLayerName = "Middle Items";
			lightLantern.GetComponent<SpriteRenderer>().sortingLayerName = "Middle Items";


		}
		if (other.gameObject.tag == "Circular") {
			print ("Circular");

		}

	}
	public void completeCall(){
		//AUDIOOOOOOOOOOO
		if (Application.loadedLevelName == "Level3") {
			if (GameObject.Find ("Circle") && GameObject.Find ("Explanation") && GameObject.Find ("ExpText")) {
				GameObject.Find ("Circle").SetActive (false);
				GameObject.Find ("Explanation").SetActive (false);
				GameObject.Find ("ExpText").SetActive (false);
			}
		}
		if(AudioController.steps.isPlaying)
		AudioController.steps.Stop ();
		if(		AudioController.magicLanternMusic.isPlaying)
			AudioController.magicLanternMusic.Stop ();

		if (levelsekarang > 30)
			AudioController.liftMoto.Play ();
		else
		AudioController.lifeOfRiley.Play ();

		if(AudioController.walkingMusic.isPlaying)
		AudioController.walkingMusic.Stop ();
		if (AudioController.ossuaryRest.isPlaying)
			AudioController.ossuaryRest.Stop ();

		isComplete = true;
		skipButt.SetActive (false);
	

		CompleteScreen.SetActive(true);
		stars[i].SetActive(true);
		lightLantern.SetActive (false);
		if ((i + 1) > PlayerPrefs.GetInt ("Level" + levelsekarang, 0)) {
			PlayerPrefs.SetInt ("Level" + levelsekarang, (i + 1));
		}
		stars[3].SetActive(false);
		
		if (showAd){   
			GetComponent<ChartBoostCall>().ShowInterstitial();
		}
	}
}


