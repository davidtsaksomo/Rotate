using UnityEngine;
using System.Collections;

public class RotateGear : MonoBehaviour {
	//rotating properties
	public bool pointNeeded;
	private float rotationSpeed = 1.5f;
	public float degree;
	private Vector3 theSpeed;
	public bool isDragging = false;
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
	private Vector3 targetPos;
	//general properties
	public bool affectAnother = false;
	public GameObject affected;
	public bool isaffected = false;

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
	public float correctDegree2 = 5454f;

	public float correctX;
	public float correctY;
	public GameObject notificationManager;
	bool slideiscorrect;
	float roundedX;
	float roundedY;

	public float domainDegree;
	public float falseDegree = 10;
	//double tap properties
	float _doubleTapTimeD;
	float touchDuration;
	Touch touch;

	//Audio Properties
	Vector3 lasttruePosition;
	Vector3 deltatruePosition;
	public bool collided;
	Vector3 deltatrueAngle;
	Vector3 lasttrueAngle;

	bool ispressed = false;
	void Start(){
		myCam = GameObject.Find ("Main Camera").GetComponent<Camera>();
		notificationManager = GameObject.Find ("NotificationManager");

		//complete thelevelnow
		NotificationCenter.DefaultCenter().AddObserver(this, "completeTheLevelNow");



		//for movement counter
		lastDegree = degree;
		lastposition = targetPos;
		lasttrueAngle = transform.eulerAngles;


		if (sliderange == 0f) {
			slideiscorrect = true;
		}

		#if UNITY_STANDALONE_WIN

		rotationSpeed = 10f;

#endif
		initialPos = transform.position;
		maxPos = new Vector3(transform.position.x + horizontalRange, transform.position.y + verticalRange, transform.position.z);
		//Using playerprefs so if player complete the level the stair can't be rotated again
		PlayerPrefs.SetInt ("Complete", 1);

	}


#if UNITY_STANDALONE_WIN
	void OnMouseDown() {

		isDragging = true;
	}
	#endif
	
	void Update() {

		#if UNITY_STANDALONE_WIN

		//detect double tap, if yes then slide
		bool doubleTapD = false;
		#region doubleTapD
		if (Input.GetMouseButtonDown (0) && isDragging == true)
		{
			if (Time.time < _doubleTapTimeD + .3f)
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







		//convert the mouse position
		mousePos = myCam.ScreenToWorldPoint (Input.mousePosition);

		//condition: isDragging, mouse inputted
		if (Input.GetMouseButton (0) && isDragging && PlayerPrefs.GetInt("Complete", 1) == 1) {
			//Define the distance of mouse input and center point of stair


			GetComponent<SpriteRenderer>().color = new Color(0.7f,0.7f,0.7f, 255);

			if(Input.GetMouseButtonDown(0)){
				domainDegree = degree;
			}

			//drag action
			if(isSliding){

				if(deltatruePosition.sqrMagnitude != 0)
					AudioController.chainDrag.volume = 1;
				else 							AudioController.chainDrag.volume = 0;
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
						
						AudioController.chainDrag.volume = 1;
					else 							AudioController.chainDrag.volume = 0;

				}
			}
		} else {
			if (isDragging) {
				theSpeed = new Vector3(0,0,0);
				isDragging = false;
				isSliding = false;
				isRotating = false;
				AudioController.chainDrag.volume = 0;
				if(degree!=domainDegree)
				{NotificationCenter.DefaultCenter().PostNotification(this, "MoveCount");}
				GetComponent<SpriteRenderer>().color = Color.white;

				if( falseDegree == degree){
					collided = true;
					degree = domainDegree;
					AudioController.wrongSound.Play();

				}
				falseDegree = 10;
			}
		}
		#endif

		//transform.Rotate(Camera.main.transform.right * theSpeed.y * rotationSpeed, Space.World);
		#if UNITY_ANDROID

		if(Input.touchCount > 0){

			//detect the double tap. If yes then slide









			touchPos = myCam.ScreenToWorldPoint (Input.GetTouch(0).position);
			if (Input.GetTouch (0).phase == TouchPhase.Began) {
				hitInfo = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position), Vector2.zero);
				// RaycastHit2D can be either true or null, but has an implicit conversion to bool, so we can use it like this
			}
			if(hitInfo){
			if((hitInfo.transform.gameObject.name == transform.gameObject.name ) && Input.touchCount > 0 && PlayerPrefs.GetInt("Complete", 1) == 1)
			{    



					if(Input.GetTouch (0).phase == TouchPhase.Began){
						domainDegree = degree;
						GetComponent<SpriteRenderer>().color = new Color(0.7f,0.7f,0.7f, 255);

					}
					ispressed = true;
				touchDuration += Time.deltaTime;
				touch = Input.GetTouch(0);
				
				if(touch.phase == TouchPhase.Began && touchDuration < 0.2f) //making sure it only check the touch once && it was a short touch/tap and not a dragging.
					StartCoroutine ("singleOrDouble");
				//if close to center, choses drag action
				//isRotating is to prevent User to switch to drag mode from rotate mode
				if(isSliding == true){
						if(deltatruePosition.sqrMagnitude != 0)
							AudioController.chainDrag.volume = 1;
						else
							AudioController.chainDrag.volume = 0;

					theSpeed = Vector3.zero;
				//	transform.rotation =  Quaternion.Euler (0, 0, degree);
					if(vertically && !diagonally){
						transform.position = new Vector3 (transform.position.x, transform.position.y+Input.GetTouch(0).deltaPosition.y/20, 0);
						
					}
					//horizontal type
					if(!vertically &&!diagonally){
						transform.position = new Vector3 (transform.position.x + Input.GetTouch(0).deltaPosition.x/20, transform.position.y, 0);
					}
					//diagonal type
					if(diagonally && upordown){
						transform.position = new Vector3 (transform.position.x + (Input.GetTouch(0).deltaPosition.x+Input.GetTouch(0).deltaPosition.y)/40, transform.position.y+(Input.GetTouch(0).deltaPosition.x+Input.GetTouch(0).deltaPosition.y)/40, 0);
						
					}
					if(diagonally && !upordown){
						transform.position = new Vector3 (transform.position.x + (Input.GetTouch(0).deltaPosition.x-Input.GetTouch(0).deltaPosition.y)/40, transform.position.y-(Input.GetTouch(0).deltaPosition.x-Input.GetTouch(0).deltaPosition.y)/40, 0);
						
					}
				}

				else{if(canRotate ){isRotating = true;

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
								
								AudioController.chainDrag.volume = 1;
							else 							AudioController.chainDrag.volume = 0;

			}
				}
			
			}
			}
			if (Input.GetTouch (0).phase == TouchPhase.Ended){
				isRotating = false;
				isSliding = false;
				theSpeed = Vector3.zero;
				AudioController.chainDrag.volume = 0;
				if(degree!=domainDegree && ispressed)
				{NotificationCenter.DefaultCenter().PostNotification(this, "MoveCount");}
				if( falseDegree == degree){
					collided = true;
					degree = domainDegree;
					AudioController.wrongSound.Play();
				}
				GetComponent<SpriteRenderer>().color = Color.white;

				ispressed = false;
				falseDegree = 10;
			}

		}
		else
		{touchDuration = 0.0f;
			}

		#endif

		transform.Rotate(Camera.main.transform.forward * (theSpeed.x + theSpeed.y) * rotationSpeed, Space.World);
		//slider limitation
		if (diagonally) {
			if (rightandup) {
				if (transform.position.x < initialPos.x) {
					transform.position = initialPos;
				}
				if (transform.position.x > maxPos.x) {
					transform.position = maxPos;
				}
				if (transform.position.x - initialPos.x < horizontalRange / 2 ) {
					targetPos = initialPos;
				}
				if (transform.position.x - initialPos.x > horizontalRange / 2) {
					targetPos = maxPos;
				}
			} else {
				if (transform.position.x > initialPos.x) {
					transform.position = initialPos;
				}
				if (transform.position.x < maxPos.x ) {
					transform.position = maxPos;
				}
				if (transform.position.x - initialPos.x > horizontalRange / 2 ) {
					targetPos = initialPos;
				}
				if (transform.position.x - initialPos.x < horizontalRange / 2 ) {
					targetPos = maxPos;
				}
				
			}
		} 



		else {
			if (rightandup) {
				if (transform.position.x < initialPos.x || transform.position.y < initialPos.y) {
					transform.position = initialPos;
				}
				if (transform.position.x > maxPos.x || transform.position.y > maxPos.y) {
					transform.position = maxPos;
				}
				if (transform.position.x - initialPos.x < horizontalRange / 2 || transform.position.y - initialPos.y < verticalRange / 2) {
					targetPos = initialPos;
				}
				if (transform.position.x - initialPos.x > horizontalRange / 2 || transform.position.y - initialPos.y > verticalRange / 2) {
					targetPos = maxPos;
				}
			} else {
				if (transform.position.x > initialPos.x || transform.position.y > initialPos.y) {
					transform.position = initialPos;
				}
				if (transform.position.x < maxPos.x || transform.position.y < maxPos.y) {
					transform.position = maxPos;
				}
				if (transform.position.x - initialPos.x > horizontalRange / 2 || transform.position.y - initialPos.y > verticalRange / 2) {
					targetPos = initialPos;
				}
				if (transform.position.x - initialPos.x < horizontalRange / 2 || transform.position.y - initialPos.y < verticalRange / 2) {
					targetPos = maxPos;
				}
			
			}
		}
		//rotation limitation
		if (PlayerPrefs.GetInt ("Complete", 1) == 1 &&!collided && !isaffected) {

			if (transform.eulerAngles.z > 0 && transform.eulerAngles.z < 45 ) {
				degree = 0f;
			}
			if (transform.eulerAngles.z > 45 && transform.eulerAngles.z < 90 ) {
				degree = 90f;
			}
			if (transform.eulerAngles.z > 90 && transform.eulerAngles.z < 135 ) {
				degree = 90f;
			}
			if (transform.eulerAngles.z > 135 && transform.eulerAngles.z < 180 ) {
				degree = 180f;
			}
			if (transform.eulerAngles.z > 180 && transform.eulerAngles.z < 225 ) {
				degree = 180f;
			}
			if (transform.eulerAngles.z > 225 && transform.eulerAngles.z < 270 ) {
				degree = 270f;
			}
			if (transform.eulerAngles.z > 270 && transform.eulerAngles.z < 315 ) {
				degree = 270f;
			}
			if (transform.eulerAngles.z > 315 && transform.eulerAngles.z < 360 ) {
				degree = 0f;
			}
		}


		if ((Input.touchCount == 0 && !isaffected) && isDragging == false) {
			if(canRotate){transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.Euler (0, 0, degree), Time.deltaTime * 10f);}
			if (verticalRange != 0 || horizontalRange != 0) {
				transform.position = Vector3.MoveTowards (transform.position, targetPos, 5f * Time.deltaTime);
			}
		}

		
		//audio penentu
		deltatruePosition = transform.position - lasttruePosition;
		lasttruePosition = transform.position;

		deltatrueAngle = transform.eulerAngles - lasttrueAngle;
		lasttrueAngle = transform.eulerAngles;

		//movement counter
		deltaPosition = targetPos - lastposition;
		if (deltaPosition.magnitude != 0 && !isaffected) {
	         movecount = !movecount;		
		}
		lastposition = targetPos;
		//movement counter: rotation
	//	deltaDegree = degree - lastDegree;
	//	if (deltaDegree != 0 && !isaffected) {
		//	movecount = !movecount;		
	//	}
		//lastDegree = degree;
		//
		if (movecount && !isDragging && Input.touchCount == 0 ) {

			movecount = false;
			NotificationCenter.DefaultCenter().PostNotification(this, "MoveCount");
		}

	
		//ability to affect another stair
		if (affectAnother) {
			if (isDragging || (Input.touchCount != 0 &&(hitInfo.transform.gameObject.name == transform.gameObject.name )  && PlayerPrefs.GetInt("Complete", 1) == 1)) {
				affected.GetComponent<Rotate> ().isaffected = true;
				affected.GetComponent<Rotate> ().degree = degree;
				affected.transform.localRotation = transform.localRotation;

			}

			else if(affected.GetComponent<Rotate> ().degree != degree){
			}

			else if(affected.GetComponent<Rotate> ().isaffected && deltaDegree == 0) {				
				affected.GetComponent<Rotate> ().isaffected = false;
			}
		
		}

		//correct or not checker
		if (sliderange != 0f) {
			roundedX = Mathf.Round(transform.position.x *1000)/1000;
			roundedY = Mathf.Round(transform.position.y *1000)/1000;
			if(correctX == roundedX && roundedY == correctY){
				slideiscorrect = true;
			}
			else{slideiscorrect = false;}

		}



		if (!isDragging && Input.touchCount == 0 && pointNeeded) {
			if ((degree == correctDegree ||degree == correctDegree2)  && slideiscorrect) {
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
				domainDegree = 10;
			}
			if (domainDegree != 10) {
				if (deltatrueAngle.sqrMagnitude < 1 && deltatruePosition.sqrMagnitude == 0)
					domainDegree = 10;
			}
	
		}
		
	}
	
	
	//double tap check

		IEnumerator singleOrDouble(){
		if (touch.tapCount == 1) {

		yield return new WaitForSeconds(0.8f);
			theSpeed = Vector3.zero;
		}
		else if(touch.tapCount == 2){
			//this coroutine has been called twice. We should stop the next one here otherwise we get two double tap

			isSliding = true;
		
			StopCoroutine("singleOrDouble");
		
		}
	}
	void completeTheLevelNow (Notification notification) {
		if (sliderange != 0) {
			transform.position = new Vector2(correctX,correctY);
		}
		degree = correctDegree;
		PlayerPrefs.SetInt ("Complete", 0);

	}


}