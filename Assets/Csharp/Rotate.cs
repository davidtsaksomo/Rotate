using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {
	public float correctX;
	public float correctY;
	//rotating properties
	public bool pointNeeded;
	public float rotationSpeed = 3f;
	public float degree;
	private Vector3 theSpeed;
	private bool isDragging = false;
	public float maxangle;
	public float maxeuler;
	public bool isRotating = false;
	public bool canRotate = true;
	//sliding properties
	public bool isSliding = false;
	public bool vertically;
	public bool diagonally;
	public float sliderange = 10.05f;
	private Vector3 initialPos;
	public float verticalRange;
	public float horizontalRange;
	public bool rightandup;
	public bool upordown;
	private Vector3 maxPos;
	public Vector3 targetPos;
	Vector3 startPos;
	//general properties
	public bool affectAnother = false;
	public GameObject affected;
	public GameObject affected2;
	public GameObject affected3;
	public GameObject affected4;
	public bool isaffected = false;
	public bool counterAffect = false;
	public bool counterAffect2 = false;
	public bool counterAffect3 = false;
	public bool counterAffect4 = false;

	float z;
	float asin;

	private Vector3 mousePos;	
	private Vector3 touchPos;
	private Vector3 lastposition;
	private Vector3 deltaPosition;
	float rotz;
	public Camera myCam;
	float lastDegree;
	float deltaDegree;
	bool movecount = false;
	private RaycastHit2D hitInfo;

	//correct or not
	bool correctornot;
	public float correctDegree;

	public GameObject notificationManager;
	bool slideiscorrect;
	float roundedX;
	float roundedY;

	//CircleGear Properties
	public bool isachild;

	//double tap properties
	float _doubleTapTimeD;
	float touchDuration;
	Touch touch;

	//Audio Properties
	Vector3 lasttruePosition;
	Vector3 deltatruePosition;
	Vector3 lasttrueAngle;
	Vector3 deltatrueAngle;
	//domainDegree
	float domainDegree;
	Vector3 domainPos;
	GameObject geartoRotate;
	RotateGear rotate;
	public GameObject xStair;
	//rigidbody properties
public bool collided;
	public GameObject parentStair;
	public bool collideonce;
	//Vector3 Input.GetTouch(0).deltaPosition.x;
	//Vector3 Input.GetTouch(0).deltaPosition.y;


	public bool xstair;
	public bool xDetermine;


	//mentok properties
	bool mentokKiri;
	bool mentokKanan;
	public GameObject mentok;
	public bool yellowMentok;

	bool wrongSound;

	void Start(){
		#if UNITY_ANDROID
		rotationSpeed = rotationSpeed * 800 / Screen.height;
		#endif
		mentok = transform.GetChild (1).gameObject;

		myCam = GameObject.Find ("Main Camera").GetComponent<Camera>();
		notificationManager = GameObject.Find ("NotificationManager");
		//complete thelevelnow
		NotificationCenter.DefaultCenter().AddObserver(this, "completeTheLevelNow");

		//movement counter initialization
		lastDegree = degree;
		lastposition = targetPos;
		lasttruePosition = transform.position;
		lasttrueAngle = transform.eulerAngles;
		// check if the stairs can sllide or not(for completion)
		if (sliderange == 0f) {
			slideiscorrect = true;
		}
		//parentStair
		if (parentStair)
			rotate = parentStair.GetComponent<RotateGear> ();


		//to adjust the speed in editor and android
		#if UNITY_STANDALONE_WIN
		rotationSpeed = 10f;

#endif
		//initial and maxpos used in sliding properties
		startPos = transform.position;
		initialPos = transform.position;

		maxPos = new Vector3(transform.position.x + horizontalRange, transform.position.y + verticalRange, transform.position.z);
		//Using playerprefs so if player complete the level the stair can't be rotated again




		//GearFront Effect
		if (gameObject.name == "_StairWhole" || gameObject.name == "_Stairhalf" || gameObject.name == "_Stairhalf 3" || gameObject.name == "_StairQuarter" || gameObject.name == "_StairQuarter 3" || gameObject.name == "_StairQuarter 6" || gameObject.name == "_StairQuater 9" || gameObject.name == "_StairQuarter360left" || gameObject.name == "_StairQuarter360left 3") {
			geartoRotate = GameObject.Find("GearFront1");
		}
		if (gameObject.name == "_StairWhole" || gameObject.name == "_Stairhalf 1" || gameObject.name == "_Stairhalf 4" || gameObject.name == "_StairQuarter 1" || gameObject.name == "_StairQuarter 4" || gameObject.name == "_StairQuarter 7" || gameObject.name == "_StairQuater 10" || gameObject.name == "_StairQuarter360left 1"  || gameObject.name == "_StairQuarter360left 4") {
			geartoRotate = GameObject.Find("GearFront2");
		}
		if (gameObject.name == "_StairWhole" || gameObject.name == "_Stairhalf 3" || gameObject.name == "_Stairhalf 5" || gameObject.name == "_StairQuarter 2" || gameObject.name == "_StairQuarter 5" || gameObject.name == "_StairQuarter 8" || gameObject.name == "_StairQuater 11" || gameObject.name == "_StairQuarter360left 2" || gameObject.name == "_StairQuarter360left 5") {
			geartoRotate = GameObject.Find("GearFront3");
		}


	}


#if UNITY_STANDALONE_WIN
	void OnMouseDown() {

		isDragging = true;
	}
	#endif
	
	void Update() {
		//Color Properties
		if (GetComponent<SpriteRenderer> ().color != Color.white && Input.touchCount == 0 && isDragging == false) {
			GetComponent<SpriteRenderer>().color = Color.white;
		}


		#if UNITY_STANDALONE_WIN

		//detect double tap, if yes then slide
		bool doubleTapD = false;
		#region doubleTapD
		if (Input.GetMouseButtonDown (0) && isDragging == true && !isachild)
		{
			if (Time.time < _doubleTapTimeD + .4f)
			{
				doubleTapD = true;
			}
			_doubleTapTimeD = Time.time;
		}
		#endregion
		if (doubleTapD)
		{
			isSliding = true;
		}
		////////////////////


		//convert the mouse position
		mousePos = myCam.ScreenToWorldPoint (Input.mousePosition);

		//condition: isDragging, mouse inputted
		if (Input.GetMouseButton (0) && isDragging && PlayerPrefs.GetInt("Complete", 1) == 1) {

		
			if (Input.GetMouseButtonDown(0)){
			domainDegree = degree;
				domainPos = targetPos;
				GetComponent<SpriteRenderer>().color = new Color(0.7f,0.7f,0.7f, 255);

			}

			//Define the distance of mouse input and center point of stair
			//if close to center, choses drag action

	

			//drag action
			if(isSliding ){
				if(deltatruePosition.sqrMagnitude != 0)
				AudioController.stairHum.volume = 1;
				else 							AudioController.stairHum.volume = 0;

				//vertical type
				if(vertically && !diagonally){
					transform.position = new Vector3 (transform.position.x, transform.position.y + (Input.GetAxis ("Mouse Y")/3), 0);

				}
				//horizontal type
				if(!vertically &&!diagonally){
					transform.position = new Vector3 (transform.position.x + (Input.GetAxis ("Mouse X")/3), transform.position.y, 0);
					}
				//diagonal type
				if(diagonally && upordown){
					transform.position = new Vector3 (transform.position.x + ((Input.GetAxis ("Mouse X")+Input.GetAxis ("Mouse Y"))/7.5f), transform.position.y + ((Input.GetAxis ("Mouse X")+Input.GetAxis ("Mouse Y"))/7.5f), 0);

				}
				if(diagonally && !upordown){
					transform.position = new Vector3 (transform.position.x + ((Input.GetAxis ("Mouse X")-Input.GetAxis ("Mouse Y"))/7.5f), transform.position.y - ((Input.GetAxis ("Mouse X")-Input.GetAxis ("Mouse Y"))/7.5f), 0);
					
				}
			
			}
			//rotate action
			else {
				if(canRotate){ isRotating = true; 
			if(mousePos.x - transform.position.x > 0 && mousePos.y - transform.position.y > 0){
			theSpeed = new Vector3 (-Input.GetAxis ("Mouse X"), Input.GetAxis ("Mouse Y"), 0.0F);
			}
			if(mousePos.x - transform.position.x > 0 && mousePos.y - transform.position.y < 0){
				theSpeed = new Vector3 (Input.GetAxis ("Mouse X"), Input.GetAxis ("Mouse Y"), 0.0F);
			}
			if(mousePos.x - transform.position.x < 0 && mousePos.y - transform.position.y > 0){
				theSpeed = new Vector3 (-Input.GetAxis ("Mouse X"), -Input.GetAxis ("Mouse Y"), 0.0F);
			}
			if(mousePos.x - transform.position.x < 0 && mousePos.y - transform.position.y < 0){
				theSpeed = new Vector3 (Input.GetAxis ("Mouse X"), -Input.GetAxis ("Mouse Y"), 0.0F);
				}
					if(theSpeed.sqrMagnitude != 0)

					AudioController.stairHum.volume = 1;
					else 							AudioController.stairHum.volume = 0;

				}
			}
		} else {
			if (isDragging) {

				AudioController.stairHum.volume = 0;
				theSpeed = new Vector3(0,0,0);
				isDragging = false;
				isSliding = false;
				isRotating = false;
				initialPos = startPos;
				maxPos = new Vector3(startPos.x + horizontalRange, startPos.y + verticalRange, transform.position.z);
				collideonce = false;
				GetComponent<SpriteRenderer>().color = Color.white;
			if(collided){
					if(wrongSound)
					AudioController.wrongSound.Play();
				}

			}
		}

		#endif

		//transform.Rotate(Camera.main.transform.right * theSpeed.y * rotationSpeed, Space.World);

		//now for android
		#if UNITY_ANDROID
		if(Input.touchCount > 0){


			//////////////

			touchPos = myCam.ScreenToWorldPoint (Input.GetTouch(0).position);
			if (Input.GetTouch (0).phase == TouchPhase.Began) {
				hitInfo = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position), Vector2.zero);

			
				// RaycastHit2D can be either true or null, but has an implicit conversion to bool, so we can use it like this
			}
			if (hitInfo){
			if((hitInfo.transform.gameObject.name == transform.gameObject.name ) && Input.touchCount > 0 && PlayerPrefs.GetInt("Complete", 1) == 1)
				{			
					
				//detect the double tap. If yes then slide
					if (Input.GetTouch (0).phase == TouchPhase.Began) {
					domainDegree = degree;
					domainPos = targetPos;
						GetComponent<SpriteRenderer>().color = new Color(0.7f,0.7f,0.7f, 255);
						print (domainDegree);
					}




				touchDuration += Time.deltaTime;
				touch = Input.GetTouch(0);
				
				if(touch.phase == TouchPhase.Began && touchDuration < 0.2f && !isachild) //making sure it only check the touch once && it was a short touch/tap and not a dragging.
					StartCoroutine("singleOrDouble");



				//if close to center, choses drag action
				//isRotating is to prevent User to switch to drag mode from rotate mode
				if((isSliding == true )){
					//	Input.GetTouch(0).deltaPosition.x = Mathf.Clamp(Input.GetTouch(0).deltaPosition.x, -10f, 10f);
						//Input.GetTouch(0).deltaPosition.y = Mathf.Clamp(Input.GetTouch(0).deltaPosition.y, -10f, 10f);

						if(deltatruePosition.sqrMagnitude != 0)
							AudioController.stairHum.volume = 1;
						else
							AudioController.stairHum.volume = 0;

					if(vertically && !diagonally){
						transform.position = new Vector3 (transform.position.x, transform.position.y+Mathf.Clamp(Input.GetTouch(0).deltaPosition.y,-10f,10f)/30, 0);
						
					}
					//horizontal type
					if(!vertically &&!diagonally){
						transform.position = new Vector3 (transform.position.x + Mathf.Clamp(Input.GetTouch(0).deltaPosition.x,-10f,10f)/30, transform.position.y, 0);
					}
					//diagonal type
					if(diagonally && upordown){
						transform.position = new Vector3 (transform.position.x + (Mathf.Clamp(Input.GetTouch(0).deltaPosition.x,-10f,10f)+Mathf.Clamp(Input.GetTouch(0).deltaPosition.y,-10f,10f))/60, transform.position.y+(Mathf.Clamp(Input.GetTouch(0).deltaPosition.x,-10f,10f)+Mathf.Clamp(Input.GetTouch(0).deltaPosition.y,-10f,10f))/60, 0);
						
					}
					if(diagonally && !upordown){
						transform.position = new Vector3 (transform.position.x + (Mathf.Clamp(Input.GetTouch(0).deltaPosition.x,-10f,10f)-Mathf.Clamp(Input.GetTouch(0).deltaPosition.y,-10f,10f))/60, transform.position.y-(Mathf.Clamp(Input.GetTouch(0).deltaPosition.x,-10f,10f)-Mathf.Clamp(Input.GetTouch(0).deltaPosition.y,-10f,10f))/60, 0);
						
					}
				}

				else{if(canRotate){isRotating = true;

				//rotating action
				if(touchPos.x - transform.position.x > 0 && touchPos.y - transform.position.y > 0){
						theSpeed = new Vector3 (-Input.GetTouch(0).deltaPosition.x, Input.GetTouch(0).deltaPosition.y, 0.0F);
				}
				if(touchPos.x - transform.position.x > 0 && touchPos.y - transform.position.y < 0){
					theSpeed = new Vector3 (Input.GetTouch(0).deltaPosition.x, Input.GetTouch(0).deltaPosition.y, 0.0F);
				}
				if(touchPos.x - transform.position.x < 0 && touchPos.y - transform.position.y > 0){
					theSpeed = new Vector3 (-Input.GetTouch(0).deltaPosition.x, -Input.GetTouch(0).deltaPosition.y, 0.0F);
				}
				if(touchPos.x - transform.position.x < 0 && touchPos.y - transform.position.y < 0){
					theSpeed = new Vector3 (Input.GetTouch(0).deltaPosition.x, -Input.GetTouch(0).deltaPosition.y, 0.0f);
				}
							if(theSpeed.sqrMagnitude != 0)
							AudioController.stairHum.volume = 1;
							else 							AudioController.stairHum.volume = 0;


			}
				}
				if (Input.GetTouch (0).phase == TouchPhase.Ended){

							
						initialPos = startPos;
						maxPos = new Vector3(startPos.x + horizontalRange, startPos.y + verticalRange, transform.position.z);
						collideonce = false;

					AudioController.stairHum.volume = 0;
						if(collided)					AudioController.wrongSound.Play();

					isRotating = false;
					isSliding = false;
						GetComponent<SpriteRenderer>().color = Color.white;

				}
			}
			}
		

		}
		else
			touchDuration = 0.0f;
		#endif
		//rotating via thespeed
		transform.Rotate (Camera.main.transform.forward * Mathf.Clamp((theSpeed.x + theSpeed.y),-8.33f,8.33f) * rotationSpeed, Space.World);
		if((theSpeed.x + theSpeed.y)>2.5f)
		print ((theSpeed.x + theSpeed.y));
		//kalo mentok beri tanda
		if (mentokKanan) {

			if (yellowMentok) {
				mentok.GetComponent<SpriteRenderer> ().flipY = false;
			}
			mentok.transform.localScale = new Vector3 (mentok.transform.localScale.x, mentok.transform.localScale.y + Mathf.Clamp ((theSpeed.sqrMagnitude), 0, 0.3f), mentok.transform.localScale.z);
		
		} else {
		}

		if(mentokKiri){
			if (yellowMentok) {
				mentok.GetComponent<SpriteRenderer> ().flipY = false;


			} 
				mentok.transform.localScale = new Vector3 (mentok.transform.localScale.x, mentok.transform.localScale.y + Mathf.Clamp ((theSpeed.sqrMagnitude), 0, 0.3f), mentok.transform.localScale.z);

			}
		else {
		}

		//rotating GeartoRotate
		if(geartoRotate)
			geartoRotate.transform.Rotate (Camera.main.transform.forward * (theSpeed.x + theSpeed.y)/2 * rotationSpeed, Space.World);

		//sliding properties limitation
		if (!isachild&&!isaffected) {
			if (diagonally) {
				if (rightandup) {
					if (transform.position.x < initialPos.x) {
						transform.position = initialPos;
					}
					if (transform.position.x > maxPos.x) {
						transform.position = maxPos;
					}
					if (transform.position.x - initialPos.x < horizontalRange / 2 &&!collided) {
						targetPos = initialPos;
					}
					if (transform.position.x - initialPos.x > horizontalRange / 2 &&!collided) {
						targetPos = maxPos;
					}
				} else {
					if (transform.position.x > initialPos.x) {
						transform.position = initialPos;
					}
					if (transform.position.x < maxPos.x) {
						transform.position = maxPos;
					}
					if (transform.position.x - initialPos.x > horizontalRange / 2&&!collided) {
						targetPos = initialPos;
					}
					if (transform.position.x - initialPos.x < horizontalRange / 2&&!collided) {
						targetPos = maxPos;
					}
				
				}
			} else {
				if (rightandup) {
					if (transform.position.x < initialPos.x || transform.position.y < initialPos.y) {
						transform.position = initialPos;
					}
					if (transform.position.x > maxPos.x || transform.position.y > maxPos.y) {
						transform.position = maxPos;
					}
					if (!collided &&(transform.position.x - initialPos.x < horizontalRange / 2 || transform.position.y - initialPos.y < verticalRange / 2)) {
						targetPos = initialPos;
					}
					if (!collided && (transform.position.x - initialPos.x > horizontalRange / 2 || transform.position.y - initialPos.y > verticalRange / 2&&!collided)) {
						targetPos = maxPos;
					}
				} else {
					if (transform.position.x > initialPos.x || transform.position.y > initialPos.y) {
						transform.position = initialPos;
					}
					if (transform.position.x < maxPos.x || transform.position.y < maxPos.y) {
						transform.position = maxPos;
					}
					if (!collided &&(transform.position.x - initialPos.x > horizontalRange / 2 || transform.position.y - initialPos.y > verticalRange / 2&&!collided)) {
						targetPos = initialPos;
					}
					if (!collided &&(transform.position.x - initialPos.x < horizontalRange / 2 || transform.position.y - initialPos.y < verticalRange / 2&&!collided)) {
						targetPos = maxPos;
					}
			
				}
			}
		}
		//rotation limitation
		if (PlayerPrefs.GetInt ("Complete", 1) == 1 && canRotate) {
			if (transform.localRotation.z <= -0.38246) {
				transform.rotation = Quaternion.Euler (0, 0, -45);
				mentokKiri = true;
			} 

			if (transform.localRotation.z > -0.38245 && transform.localRotation.z < 0 && !isaffected &&!collided) {
				degree = -45f;
			}
			if (transform.localRotation.z > 0 && transform.localRotation.z < maxangle && !isaffected&&!collided) {
				degree = 45f;
			}
			if (transform.localRotation.z >= maxangle) {
				transform.rotation = Quaternion.Euler (0, 0, maxeuler);
				mentokKanan = true;
			} 
			if (mentokKanan &&theSpeed.x + theSpeed.y < 0) {
				mentokKanan = false;
				mentok.transform.localScale = new Vector3 (mentok.transform.localScale.x, 0f, mentok.transform.localScale.z);

			}
			if (mentokKiri &&theSpeed.x + theSpeed.y > 0) {
				mentokKiri = false;
				mentok.transform.localScale = new Vector3 (mentok.transform.localScale.x, 0f, mentok.transform.localScale.z);

			}

		}
		//move to the destined position
		if ((Input.touchCount == 0 && !isaffected) && isDragging == false) {
			if(canRotate){transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.Euler (0, 0, degree), Time.deltaTime * 10f);}
			if (verticalRange != 0 || horizontalRange != 0 ) {
				transform.position = Vector3.MoveTowards (transform.position, targetPos, 5f * Time.deltaTime);
			}
		}
		//movement counter
		deltaPosition = targetPos - lastposition;
		if (deltaPosition.sqrMagnitude != 0 && !isaffected) {
	         movecount = !movecount;	
		}
		lastposition = targetPos;

		//real deltaPosition
		deltatruePosition = transform.position - lasttruePosition;
		lasttruePosition = transform.position;

		deltatrueAngle = transform.eulerAngles - lasttrueAngle;
		lasttrueAngle = transform.eulerAngles;


		//movement counter: rotation
		deltaDegree = degree - lastDegree;
		if (deltaDegree != 0 && !isaffected) {
			movecount = !movecount;	
		}
		lastDegree = degree;

		if (movecount && !isDragging && Input.touchCount == 0 ) {

			movecount = false;
			NotificationCenter.DefaultCenter().PostNotification(this, "MoveCount");
		}

	
		//ability to affect another stair
		if (affectAnother ) {
			if (isDragging || (Input.touchCount != 0 &&(hitInfo.transform.gameObject.name == transform.gameObject.name ) && PlayerPrefs.GetInt("Complete", 1) == 1)) {
				if(affected){
					if(!counterAffect){
				affected.GetComponent<Rotate> ().isaffected = true;
				affected.GetComponent<Rotate> ().degree = degree;
				affected.transform.localRotation = transform.localRotation;
					}else{
						affected.GetComponent<Rotate> ().isaffected = true;
						affected.GetComponent<Rotate> ().degree = -1*degree;
						z = transform.localRotation.z * -1;
						asin = Mathf.Asin(z) * Mathf.Rad2Deg *2;
						asin = Mathf.Round(asin);
						affected.transform.localRotation = Quaternion.Euler (0,0,asin);
					}
				}
				if(affected2){
					if(!counterAffect2){
					affected2.GetComponent<Rotate> ().isaffected = true;
					affected2.GetComponent<Rotate> ().degree = degree;
					affected2.transform.localRotation = transform.localRotation;
					} else{
						affected2.GetComponent<Rotate> ().isaffected = true;
						affected2.GetComponent<Rotate> ().degree = -1*degree;
						z = transform.localRotation.z * -1;
						asin = Mathf.Asin(z) * Mathf.Rad2Deg *2;
						asin = Mathf.Round(asin);
						affected2.transform.localRotation = Quaternion.Euler (0,0,asin);
					}
				}
				if(affected3){
					if(!counterAffect3){
						affected3.GetComponent<Rotate> ().isaffected = true;
						affected3.GetComponent<Rotate> ().degree = degree;
						affected3.transform.localRotation = transform.localRotation;
					} else{
						affected3.GetComponent<Rotate> ().isaffected = true;
						affected3.GetComponent<Rotate> ().degree = -1*degree;
						z = transform.localRotation.z * -1;
						asin = Mathf.Asin(z) * Mathf.Rad2Deg *2;
						asin = Mathf.Round(asin);
						affected3.transform.localRotation = Quaternion.Euler (0,0,asin);
					}
				}
				if(affected4){
					if(!counterAffect4){
						affected4.GetComponent<Rotate> ().isaffected = true;
						affected4.GetComponent<Rotate> ().degree = degree;
						affected4.transform.localRotation = transform.localRotation;
					} else{
						affected4.GetComponent<Rotate> ().isaffected = true;
						affected4.GetComponent<Rotate> ().degree = -1*degree;
						z = transform.localRotation.z * -1;
						asin = Mathf.Asin(z) * Mathf.Rad2Deg *2;
						asin = Mathf.Round(asin);
						affected4.transform.localRotation = Quaternion.Euler (0,0,asin);
					}
				}

			}
		

			else {
				if(affected){
				if(affected.GetComponent<Rotate> ().isaffected && deltaDegree == 0) {				
				affected.GetComponent<Rotate> ().isaffected = false;
				}
				}
				if(affected2){
				if(affected2.GetComponent<Rotate> ().isaffected && deltaDegree == 0) {				
					affected2.GetComponent<Rotate> ().isaffected = false;
				}
				}
				if(affected3){
					if(affected3.GetComponent<Rotate> ().isaffected && deltaDegree == 0) {				
						affected3.GetComponent<Rotate> ().isaffected = false;
					}
				}
				if(affected4){
					if(affected4.GetComponent<Rotate> ().isaffected && deltaDegree == 0) {				
						affected4.GetComponent<Rotate> ().isaffected = false;
					}
				}
			}
		
		}

		//correct or not checker
		if (sliderange != 0f) {
			roundedX = Mathf.Round(transform.position.x *1000)/1000;
			roundedY = Mathf.Round(transform.position.y *1000)/1000;


			if(correctX == roundedX && roundedY == correctY){
				slideiscorrect = true;
			}
			else{slideiscorrect = false;
			}

		}



		if (!isDragging && Input.touchCount == 0 && pointNeeded) {
			if ((degree == correctDegree || !canRotate || xstair) && slideiscorrect) {
				if(!correctornot){
				notificationManager.GetComponent<CompleteChecker> ().amount++;
				correctornot = true;
				}
			} else if (correctornot) {
				notificationManager.GetComponent<CompleteChecker> ().amount--;
				correctornot = false;
			}
		}
		//turn collided to false
		if (!isDragging && Input.touchCount == 0) {
			if (collided) {
				if (deltatrueAngle.sqrMagnitude < 1 && deltatruePosition.sqrMagnitude == 0) 
					collided = false;
					domainDegree = 0;
					domainPos = Vector3.zero;
				}
				if (domainDegree != 0) {
					if (deltatrueAngle.sqrMagnitude < 1 && deltatruePosition.sqrMagnitude == 0)
						domainDegree = 0;
						//domainPos = Vector3.zero;
				}
				if (domainPos != Vector3.zero) {
					if (deltatrueAngle.sqrMagnitude < 1 && deltatruePosition.sqrMagnitude == 0)
				{
					domainDegree = 0;
			     	domainPos = Vector3.zero;
				}
			}
		}
		if (xStair) {
			if(isDragging || hitInfo){
			if(isDragging || hitInfo.transform.gameObject.name == transform.gameObject.name){			xStair.transform.position = transform.position;//	xStair.GetComponent<Rotate>().targetPos = domainPos;
				xDetermine = true;
				xStair.GetComponent<Rotate>().xDetermine = false;

			if(collided){xStair.GetComponent<Rotate>().collided = true;
				}
			else xStair.GetComponent<Rotate>().collided = false;
			}
			}
			if(!isDragging &&Input.touchCount == 0 && targetPos != xStair.GetComponent<Rotate>().targetPos &&!xDetermine)
			{targetPos = xStair.GetComponent<Rotate>().targetPos;
				transform.position = xStair.transform.position ;
				collided = true;
			}
		}














	}

	
	//double tap check
	IEnumerator singleOrDouble(){
		if (touch.tapCount == 1) {
			theSpeed = Vector3.zero;


		yield return new WaitForSeconds(0.8f);
		}
		else if(touch.tapCount == 2){
			//this coroutine has been called twice. We should stop the next one here otherwise we get two double tap
			isSliding = true;

			StopCoroutine("singleOrDouble");
		}
	}
	void completeTheLevelNow (Notification notification) {
		if (sliderange != 0) {
			transform.position = new Vector3(correctX,correctY, transform.position.z);
			targetPos = transform.position;
		}
		degree = correctDegree;
		PlayerPrefs.SetInt ("Complete", 0);

	}
	void OnTriggerEnter2D(Collider2D other){
		if (other.gameObject.tag == "stairs") {
			wrongSound = true;
			GetComponent<SpriteRenderer>().color = new Color(1f,0.4f,0.4f, 255);


		}
		if (other.gameObject.tag == "stairs" && domainDegree != 0 && domainDegree != degree) {
			collided = true;
			print ("d");
			wrongSound = true;
			degree = domainDegree;
			if (!isDragging && Input.touchCount == 0 && wrongSound) {
				AudioController.wrongSound.Play ();
			}
		}
		if (other.gameObject.tag == "stairs" && isachild && rotate.domainDegree != rotate.degree && ! rotate.collided && rotate.domainDegree != 10) {
			print ("aweu");
			if (rotate.isDragging || Input.touchCount != 0)
				rotate.falseDegree = rotate.degree;
			else {
				rotate.collided = true;
				rotate.degree = rotate.domainDegree;
				AudioController.wrongSound.Play ();
			}
		} 
		if (other.gameObject.tag == "stairs" && domainPos != Vector3.zero && domainPos != targetPos) {
			collided = true;			print ("f");

			targetPos = domainPos;
	

			collideonce = true;
			if (!isDragging && Input.touchCount == 0) {
				AudioController.wrongSound.Play ();
			}

		}
		if (other.gameObject.tag == "stairs" && !collideonce && !isachild && (isDragging || hitInfo) &&!diagonally ) {
			if (  isDragging||hitInfo.transform.gameObject.name == transform.gameObject.name  
			   ) {			print ("g");

				collideonce = true;
				//bump effect kwadran
				if(verticalRange == 2.16f || verticalRange == -2.16f || horizontalRange == 2.16f || horizontalRange == -2.16f){
					AudioController.bump.Play();

				if (targetPos == initialPos)
					maxPos = new Vector3 (initialPos.x + horizontalRange / 2.3f, initialPos.y + verticalRange / 2.3f, transform.position.z);
				if (targetPos == maxPos)
					initialPos = new Vector3 (initialPos.x + horizontalRange / 1.7f, initialPos.y + verticalRange / 1.7f, transform.position.z);
				}
			
			
			
			}
		}



	}
	void OnTriggerExit2D(Collider2D other){
		if (other.gameObject.tag == "stairs") {
			wrongSound = false;
			GetComponent<SpriteRenderer>().color = new Color(0.7f,0.7f,0.7f, 255);

		
		}
	}
		

}

