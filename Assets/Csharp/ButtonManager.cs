using UnityEngine;
using System;
using System.Text;
using UnityEngine.Advertisements;
using ChartboostSDK;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class ButtonManager : MonoBehaviour {
	public int levelsekarang;
	int levelselanjutnya;
	public GameObject paused;
	public GameObject complete;
	bool isPaused;
	public GameObject lanternLeft;
	public GameObject skipNotice;
	public GameObject loadingVideoText;
	public GameObject loadingMagicGearText;
	public GameObject proceedButt;
	public GameObject soundsAdjust;
	public GameObject loadingScreen;
	public GameObject magicGear;
	public GameObject[] opening;
	public static bool canClose = true;
	bool gear = false;
	// Use this for initialization

	void OnEnable()
	{
		// Listen to all impression-related events

		Chartboost.didFailToLoadRewardedVideo += didFailToLoadRewardedVideo;

		Chartboost.didCompleteRewardedVideo += didCompleteRewardedVideo;
		Chartboost.didDisplayRewardedVideo += didDisplayRewardedVideo;
	
	}
	void OnDisable() {
		// Remove event handlers
	
		Chartboost.didFailToLoadRewardedVideo -= didFailToLoadRewardedVideo;

		Chartboost.didCompleteRewardedVideo -= didCompleteRewardedVideo;
		Chartboost.didDisplayRewardedVideo -= didDisplayRewardedVideo;

		#if UNITY_IPHONE
		Chartboost.didCompleteAppStoreSheetFlow -= didCompleteAppStoreSheetFlow;
		#endif
	}
	void Start () {
		
		
		if (Application.loadedLevel == 1) {
			if(!AudioController.carefree.isPlaying)
			AudioController.carefree.Play ();
			AudioController.GonnaStart.Stop ();
		}

	//	if (!Advertisement.isInitialized) {
	//		Advertisement.Initialize ("56677", false);
	//		Advertisement.allowPrecache = false;
	//	}
		levelselanjutnya = levelsekarang + 1;
	}
	
	// Update is called once per frame
	void Update () {
		
		if (Input.GetKeyUp(KeyCode.Escape)){
			if(Application.loadedLevelName == "MainMenu"){
				Application.Quit();
			}
			else if(Application.loadedLevelName == "LevelSelector"){
				Home ();
			}	
			else if (complete.activeSelf == true){
				MainMenu();
			}
			else if(isPaused == false){
				pause ();
			}
			else if(isPaused == true){
				resume();
			}
		}	
	}

	public void NextLevel(){
		displayLoadingScreen ();
		AudioController.clickSound.Play ();
		Application.LoadLevelAsync ("Level" + levelselanjutnya);
	}
	public void Restart(){
		AudioController.clickSound.Play ();
		displayLoadingScreen ();

		
		Application.LoadLevelAsync (Application.loadedLevel);
	}
	public void MainMenu(){
		AudioController.clickSound.Play ();
		displayLoadingScreen ();

		Application.LoadLevelAsync ("LevelSelector");
	}
	public void credit(){
		AudioController.clickSound.Play ();
		displayLoadingScreen ();

		Application.LoadLevelAsync ("Credit");
	}
	public void exit(){
		AudioController.clickSound.Play ();

		Application.Quit ();
	}
	public void Home(){
		AudioController.clickSound.Play ();
		displayLoadingScreen ();

		Application.LoadLevelAsync ("MainMenu");

	}

	public void pause(){
		if (PlayerPrefs.GetInt ("Complete", 1) == 1) {
			AudioController.clickSound.Play ();
			paused.SetActive (true);
			PlayerPrefs.SetInt ("Complete", 0);
			isPaused = true;
			
		}
	}
	public void resume(){
		AudioController.clickSound.Play ();
		if (canClose) {
			paused.SetActive (false);
			PlayerPrefs.SetInt ("Complete", 1);
			isPaused = false;
		}

	}
	public void completeLevelNow(){
		
			AudioController.clickSound.Play ();

			skipNotice.SetActive (true);
			if (PlayerPrefs.GetInt ("MagicLantern") > 0)
				proceedButt.GetComponent<Button> ().interactable = true;
			else
				proceedButt.GetComponent<Button> ().interactable = false;

			lanternLeft.GetComponent<Text> ().text = "You Have " + PlayerPrefs.GetInt ("MagicLantern");

	
	}
	public void destroySkipNotice(){


		AudioController.clickSound.Play ();
		if (canClose) {
			skipNotice.SetActive (false);
			loadingVideoText.SetActive (false);
		}
	}
	public void displayPatientText(){
		
		AudioController.clickSound.Play ();
		if (canClose) {
			loadingVideoText.GetComponent<Text> ().text = "Please Wait...";
			loadingVideoText.SetActive (true);
		}

	}
	public void displayUnityAds(){
		
		if (canClose) {
			Chartboost.cacheRewardedVideo (CBLocation.MainMenu);
			InvokeRepeating ("showRewardVid", 0, 0.5f);
			canClose = false;
		}
	}
	void showRewardVid(){
		if (Chartboost.hasRewardedVideo(CBLocation.MainMenu)){ 
			Chartboost.showRewardedVideo(CBLocation.MainMenu);

		}
	}
	void didDisplayRewardedVideo(CBLocation location){
		CancelInvoke ("showRewardVid");
		canClose = true;
	}
	void didFailToLoadRewardedVideo(CBLocation location, CBImpressionError error) {
		CancelInvoke ("showRewardVid");
		canClose = true;

		loadingVideoText.GetComponent<Text>().text = "Video Not Available at the time";
		loadingMagicGearText.GetComponent<Text>().text = "Video Not Available at the time";
	}
	void didCompleteRewardedVideo(CBLocation location, int reward){
		if (!gear) {
			loadingVideoText.GetComponent<Text> ().text = "You got 1 Magic Lantern!";
			PlayerPrefs.SetInt ("MagicLantern", (PlayerPrefs.GetInt ("MagicLantern") + 1));
			NotificationCenter.DefaultCenter ().PostNotification (this, "LanternStatus");

			lanternLeft.GetComponent<Text> ().text = "You Have " + PlayerPrefs.GetInt ("MagicLantern");
		} else {
			loadingMagicGearText.GetComponent<Text>().text = "You got 5 Magic Gear!";
			PlayerPrefs.SetInt("MagicGear",(PlayerPrefs.GetInt("MagicGear")+5));
			NotificationCenter.DefaultCenter().PostNotification(this, "GearStatus");
			gear = false;
		}
	}

	public void displayLoadingScreen()
	{if (loadingScreen) {loadingScreen.SetActive(true);
		}
		
	}
	public void soundAdjusting (){
		soundsAdjust.SetActive (true);
		AudioController.clickSound.Play ();
	}
	public void soundOk(){
		soundsAdjust.SetActive (false);
		AudioController.clickSound.Play ();
	}
	public void skipButton(){
		AudioController.clickSound.Play ();

		if (PlayerPrefs.GetInt ("MagicGear") > 0) {
			PlayerPrefs.SetInt ("MagicGear", PlayerPrefs.GetInt ("MagicGear") - 1);
			GameObject.Find ("Character").GetComponent<Movements> ().completeCall ();
			NotificationCenter.DefaultCenter().PostNotification(this, "GearStatus");

		} else {magicGear.SetActive(true);
			Time.timeScale = 0;
			AudioController.steps.Stop();
		}
	}
	public void cancelMagicGear(){
		if (canClose) {
			AudioController.clickSound.Play ();

			magicGear.SetActive (false);
			Time.timeScale = 1;
			loadingMagicGearText.SetActive (false);
			AudioController.steps.Play ();
		}
	}
	public void displayloadingGearText(){
		AudioController.clickSound.Play ();

			loadingMagicGearText.GetComponent<Text> ().text = "Please Wait...";
			loadingMagicGearText.SetActive (true);
		
	}
	public void displayMagicGearAds(){
		
		if (canClose) {
			Chartboost.cacheRewardedVideo (CBLocation.MainMenu);
			InvokeRepeating ("showRewardVid", 0, 0.5f);
			canClose = false;
			gear = true;
		}
	
	}
	public void gotItButton(){
		Destroy (GameObject.Find ("HowTo"));
	}
	public void Opening(){
		//if (PlayerPrefs.GetInt ("Level1", 0) == 0) {

		//	opening [0].SetActive (true);
		//	opening [1].SetActive (true);
		//	AudioController.paper.Play ();
		//} else {
			MainMenu ();
		//}
	}

	public void ending(){
		displayLoadingScreen ();
		AudioController.clickSound.Play ();
		if (AudioController.liftMoto.isPlaying)
			AudioController.liftMoto.Stop ();
		if (AudioController.GonnaStart.isPlaying)
			AudioController.GonnaStart.Stop ();
		AudioController.carefree.Play ();
		Application.LoadLevelAsync ("Ending");
	}
	public void congratPref(){
		if (!PlayerPrefs.HasKey ("congratPref")) {
			PlayerPrefs.SetInt ("congratPref", 1);
		}
	}

}
