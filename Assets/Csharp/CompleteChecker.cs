using UnityEngine;
using UnityEngine.UI;

using Holoville.HOTween;
using Holoville.HOTween.Core;
using Holoville.HOTween.Path;
using Holoville.HOTween.Plugins;
using Holoville.HOTween.Plugins.Core;
using System.Collections;
public class CompleteChecker : MonoBehaviour {
	public int amount;
	public int needed;
	public int paths;
	public int maxMove;
	public int minMove;
	public int levelSekarang;
	public int circularStair;
	public int circularStair2;

	public GameObject moveText;
	public GameObject recordText;
	public GameObject characters;
	int i=0;
	public int moves;
	public GameObject skipButton;
	float time = 1;
	GameObject overlay;
	public Transform character;
	public GameObject lightLantern;
	// Use this for initialization
	void Awake(){		NotificationCenter.DefaultCenter().AddObserver(this, "MoveCount");
	}

	void Start () {

		PlayerPrefs.SetInt ("Complete", 1);


		if(levelSekarang > 30)
			AudioController.ossuaryRest.Play();
		else
		AudioController.backgroundMusic.Play ();

		if(AudioController.lifeOfRiley.isPlaying)
		AudioController.lifeOfRiley.Stop ();
		if (AudioController.liftMoto.isPlaying)
			AudioController.liftMoto.Stop ();
		if (AudioController.GonnaStart.isPlaying)
			AudioController.GonnaStart.Stop ();
		AudioController.stairHum.Play ();
		AudioController.chainDrag.Play ();

		
		
		//load the moves record
		if (PlayerPrefs.HasKey ("MoveRecordLevel" + levelSekarang)) {
			recordText.GetComponent<Text> ().text = "Record: " + PlayerPrefs.GetInt ("MoveRecordLevel" + levelSekarang);
		} else {
			recordText.GetComponent<Text> ().text = "Record: -";
		}


	}
	
	// Update is called once per frame
	void Update () {
		if (amount == needed ) {
			PlayerPrefs.SetInt ("Complete", 0);
			Destroy (GameObject.Find("Lantern"));
			Destroy (GameObject.Find("Tutorial"));

			if (Application.loadedLevel == 24) {
				if(GameObject.Find("Circle"))
				Destroy(GameObject.Find("Circle"));
			}
			if (Application.loadedLevel== 5) {
				if (GameObject.Find ("Circle") && GameObject.Find ("Explanation") && GameObject.Find ("ExpText")) {
					GameObject.Find ("Circle").GetComponent<Image> ().enabled = true;
					GameObject.Find ("Explanation").GetComponent<Image> ().enabled = true;
					GameObject.Find ("ExpText").GetComponent<Text> ().enabled = true;
				}
			}
			amount=0;
			Complete();
			GameObject.Find("Character").GetComponent<Animator>().SetBool("isWalking",true);
			NentuinBintang();
			record ();
			if(!AudioController.magicLanternMusic.isPlaying){
				AudioController.steps.Play();
				if(levelSekarang<31)
				AudioController.walkingMusic.Play ();

			}

			AudioController.chainDrag.Stop();
			AudioController.stairHum.Stop ();
			AudioController.success.Play ();

			AudioController.backgroundMusic.Stop();
			skipButton.SetActive(true);
		}
	
	}

	void record(){
		if(PlayerPrefs.HasKey("MoveRecordLevel"+levelSekarang)){

			if(moves < PlayerPrefs.GetInt("MoveRecordLevel"+levelSekarang)){
				PlayerPrefs.SetInt("MoveRecordLevel"+levelSekarang, moves);
			}
			   }
		else{PlayerPrefs.SetInt("MoveRecordLevel"+levelSekarang, moves);
		}

	}

	void MoveCount (Notification notification) {
		moves++;
		moveText.GetComponent<Text> ().text = "Moves: " + moves;
	}

	void NentuinBintang(){
		if (moves < minMove || moves == minMove) {
			characters.GetComponent<Movements>().i = 2;
		}
		else if (moves > maxMove) {
			characters.GetComponent<Movements>().i = 0;
		} else {
			characters.GetComponent<Movements>().i = 1;
		}
	}

	void Complete(){

		if (i == circularStair - 1 || i == circularStair2 - 1) {
			time = 3f;
		}
		else{time = 1f;}


		// HOTWEEN INITIALIZATION
		// Must be done only once, before the creation of your first tween
		// (you can skip this if you want, and HOTween will be initialized automatically
		// when you create your first tween - using default values)
		HOTween.Init( true, true, true);
		
		// TWEEN CREATION
		// With each one is shown a different way you can create a tween,
		// so that in the end you can choose what you prefer
		
		// Tween the first transform using fast writing mode,
		// and applying an animation that will last 4 seconds
		if (i < paths){ 
		
				
					HOTween.To(character , time, new TweenParms()
					           .Prop( "position", character.GetComponent<HOPath>().MakePlugVector3Path().OrientToPath(Axis.X | Axis.Y), true)
					           .Loops(1)
						       .OnComplete(Complete)
					           .Ease(EaseType.Linear));
					i++;
					Destroy (character.GetComponent<HOPath> (), 0f);


					

					
		}
	}

	public void completeTheLevelNow(){
		if (ButtonManager.canClose) {
			PlayerPrefs.SetInt ("MagicLantern", (PlayerPrefs.GetInt ("MagicLantern") - 1));
			NotificationCenter.DefaultCenter ().PostNotification (this, "LanternStatus");
			if (AudioController.ossuaryRest.isPlaying)
				AudioController.ossuaryRest.Stop ();
			NotificationCenter.DefaultCenter ().PostNotification (this, "completeTheLevelNow");
			lightLantern.SetActive (true);
			AudioController.magicLanternMusic.Play ();
			moves = minMove;
			moveText.GetComponent<Text> ().text = "Moves: " + moves;
			NotificationCenter.DefaultCenter ().RemoveObserver (this, "MoveCount");
	
			overlay = GameObject.Find ("Overlay");
			if (overlay)
				overlay.GetComponent<Animator> ().SetBool ("overlay", false);
		}
	}
	public void destroySkip(){
		skipButton.SetActive (false);
	}
}
