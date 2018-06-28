using UnityEngine;
using System.Collections;

public class AudioController : MonoBehaviour {
	public AudioSource[] sounds;
	public  static AudioSource backgroundMusic;
	public  static AudioSource walkingMusic;
	public  static AudioSource steps;
	public static AudioSource clickSound;
	public static AudioSource success;
	public static AudioSource lifeOfRiley;
	public static AudioSource stairHum;
	public static AudioSource magicLanternMusic;
	public static AudioSource chainDrag;
	public static AudioSource wrongSound;
	public static AudioSource ossuaryRest;
	public static AudioSource liftMoto;
	public static AudioSource GonnaStart;
	public static AudioSource carefree;
	public static AudioSource pickup;
	public static AudioSource bump;
	public static AudioSource splash;
	public static AudioSource paper;

	float musicvolume;
	// Use this for initialization
	void Awake () {DontDestroyOnLoad (this.gameObject);
		if (FindObjectsOfType(GetType()).Length > 1)
		{
			Destroy(gameObject);
		}



	}

	void Start (){

		sounds = GetComponents<AudioSource> ();
		backgroundMusic = sounds [0];
		steps = sounds [1];
		clickSound = sounds [2];
		success = sounds [3];
		lifeOfRiley = sounds [4];
		walkingMusic = sounds [5];
		stairHum = sounds [6];
		magicLanternMusic = sounds [7];
		stairHum.volume = 0;
		chainDrag = sounds [8];
		chainDrag.volume = 0;
		wrongSound = sounds [9];
		ossuaryRest = sounds [10];
		liftMoto = sounds [11];
		GonnaStart = sounds [12];
		carefree = sounds [13];
		pickup = sounds [14];
		bump = sounds [15];
		splash = sounds [16];
		paper = sounds [17];

		sounds [0].ignoreListenerVolume = true;
		sounds [4].ignoreListenerVolume = true;
		sounds [5].ignoreListenerVolume = true;
		sounds [7].ignoreListenerVolume = true;
		sounds [10].ignoreListenerVolume = true;
		sounds [11].ignoreListenerVolume = true;
		sounds [12].ignoreListenerVolume = true;
		sounds [13].ignoreListenerVolume = true;


		//setvolume
		musicvolume = PlayerPrefs.GetFloat ("musicVolume", 1f);
		AudioListener.volume = 		PlayerPrefs.GetFloat ("sfxVolume", 1f);
		backgroundMusic.volume = musicvolume;
		walkingMusic.volume = musicvolume;
		lifeOfRiley.volume = musicvolume;
		magicLanternMusic.volume = musicvolume;
		ossuaryRest.volume = musicvolume;
		liftMoto.volume = musicvolume;
		GonnaStart.volume = musicvolume;
		carefree.volume = musicvolume;

	}
	// Update is called once per frame
	void Update () {
	
	}

	public void sfxAdjust(float volume){
		PlayerPrefs.SetFloat ("sfxVolume", volume);
		AudioListener.volume = 		PlayerPrefs.GetFloat ("sfxVolume", 1f);

	}
	public void bgmAdjust(float volume){
		PlayerPrefs.SetFloat ("musicVolume", volume);
		musicvolume = volume;
		backgroundMusic.volume = musicvolume;
		walkingMusic.volume = musicvolume;
		lifeOfRiley.volume = musicvolume;
		magicLanternMusic.volume = musicvolume;
		ossuaryRest.volume = musicvolume;
		liftMoto.volume = musicvolume;
		GonnaStart.volume = musicvolume;
		carefree.volume = musicvolume;

	}
}
