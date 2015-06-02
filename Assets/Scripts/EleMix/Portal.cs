using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Portal : MonoBehaviour {

	public string portalId;

	public bool elementsOnly;

	public Vector3 exitPosition;
	public float exitAngle;

	public float minScale;
	public float maxScale;
	public float scaleWaveTime; // time it takes to go from size min to max and back to min

	public int rotationSpeed;

	public AudioClip departSound;
	public AudioClip arriveSound;

	// TODO: those could be publicly settable, but then a fitting sound should also be configurable
	private float departTime = .5f;
	private float arriveTime = .5f;

	private Dictionary<GameObject, float> portingObjectDistance;
	private HashSet<GameObject> portingObjects;
	private static int TOUCH_LAYER = 8;

	private Hashtable toMinAnimOptions;
	private Hashtable toMaxAnimOptions;

	private ContractWorkProgression contractWorkProgression;

	// Use this for initialization
	void Start () {
	
		if( minScale == maxScale ) {
			// let's set the portal size
			transform.localScale = new Vector3( minScale, minScale, 1);
		} else {

			StartPulsating();
		}

		portingObjectDistance = new Dictionary<GameObject, float>();
		portingObjects = new HashSet<GameObject>();

		contractWorkProgression = 
			GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<ContractWorkProgression>();
	}

	void Update() {

		transform.Rotate(Vector3.back * Time.deltaTime * rotationSpeed, Space.Self);
	}


	void StartPulsating() {

		// let's set the size half way between min and max
		float meanScale = (minScale + maxScale) / 2;
		transform.localScale = new Vector3( meanScale, meanScale, 1 );

		// and configure animations to min and max
		toMinAnimOptions = new Hashtable();
		toMinAnimOptions.Add( "scale", new Vector3( minScale, minScale, 1) );
		toMinAnimOptions.Add( "time", scaleWaveTime / 2 );
		toMinAnimOptions.Add( "easetype", iTween.EaseType.easeInOutSine );
		toMinAnimOptions.Add( "oncomplete", "ScaleToMax" );
		toMinAnimOptions.Add( "oncompletetarget", gameObject );

		toMaxAnimOptions = new Hashtable();
		toMaxAnimOptions.Add( "scale", new Vector3( maxScale, maxScale, 1) );
		toMaxAnimOptions.Add( "time", scaleWaveTime / 2 );
		toMaxAnimOptions.Add( "easetype", iTween.EaseType.easeInOutSine );
		toMaxAnimOptions.Add( "oncomplete", "ScaleToMin" );
		toMaxAnimOptions.Add( "oncompletetarget", gameObject );

		ScaleToMin();
	}
	void ScaleToMin() {
		iTween.ScaleTo( gameObject, toMinAnimOptions );
	}
	void ScaleToMax() {
		iTween.ScaleTo( gameObject, toMaxAnimOptions );
	}

	
	void OnTriggerStay( Collider other ) {

		float currentDistanceToCenter = Vector3.Distance( transform.position, other.bounds.center );
		if( portingObjectDistance.ContainsKey(other.gameObject) ) {

			if( currentDistanceToCenter < portingObjectDistance[other.gameObject] ) {
				// we have approached the portal center
				
				bool hasEnteredPortal = false;
				
				if( other.gameObject.layer == TOUCH_LAYER ) { // we have an element
					
					hasEnteredPortal = gameObject.transform.position != other.bounds.center && 
						Vector3.Distance(gameObject.transform.position, other.bounds.center) < (transform.localScale.x / 2);
					
				} else if( ! elementsOnly ) {
					
					hasEnteredPortal = true;
				}
				
				if( hasEnteredPortal && ! portingObjects.Contains(other.gameObject) ) {
					
					portingObjects.Add( other.gameObject );
					
					StartCoroutine( Porter(other.gameObject) );
				}
			}

		} else {
			portingObjectDistance.Add( other.gameObject, currentDistanceToCenter );
		}
	}

	void OnTriggerExit( Collider other ) {

		if( null != other && null != other.gameObject ) {
			portingObjectDistance.Remove( other.gameObject );
			portingObjects.Remove( other.gameObject );
		}
	}


	IEnumerator Porter( GameObject elementToPort ) {

		float departTimer = 0f;
		float arriveTimer = 0f;

		Vector3 originalScale = elementToPort.transform.localScale;
		Vector3 originalPosition = elementToPort.transform.position;
		Vector3 originalVelocity = elementToPort.GetComponent<Rigidbody>().velocity;

		GetComponent<AudioSource>().pitch = GetAudioPitchFromGameObjectScale( elementToPort );
		GetComponent<AudioSource>().PlayOneShot( departSound, 1.0f );

		elementToPort.GetComponent<Rigidbody>().velocity = Vector3.zero;

		TrailRenderer trail = elementToPort.GetComponent<TrailRenderer>();

		while( departTimer < departTime ) {

			departTimer += Time.deltaTime;
			float timeFraction = departTimer / departTime;

			elementToPort.transform.localScale = Vector3.Lerp( originalScale, Vector3.zero, timeFraction );

			elementToPort.transform.position = Vector3.Lerp( originalPosition, transform.position, timeFraction );


			if( null != trail ) trail.startWidth = elementToPort.transform.localScale.x;

			yield return null;
		}

		if( exitPosition != Vector3.zero ) { // so we're not a black hole, but a portal!
			elementToPort.transform.position = exitPosition;
			originalVelocity = Quaternion.AngleAxis( exitAngle, Vector3.up ) * originalVelocity;
			
			GetComponent<AudioSource>().PlayOneShot( arriveSound, 1.0f );
			while( arriveTimer < arriveTime ) {
				
				arriveTimer += Time.deltaTime;
				float timeFraction = arriveTimer / arriveTime;

				elementToPort.transform.localScale = Vector3.Lerp( Vector3.zero, originalScale, timeFraction );

				elementToPort.GetComponent<Rigidbody>().velocity = Vector3.Lerp( Vector3.zero, originalVelocity, timeFraction );
			

				if( null != trail ) trail.startWidth = elementToPort.transform.localScale.x;

				yield return null;
			}

			portingObjects.Remove( elementToPort );

		} else {

			portingObjects.Remove( elementToPort );

			contractWorkProgression.QuitPlayIfElementTagCountIsBelowThreshold( elementToPort.tag, 3 );

			Destroy( elementToPort );
		}
	}


	float GetAudioPitchFromGameObjectScale( GameObject audioTrigger ) {

		float pitch = 0f;

		if( audioTrigger.transform.localScale.x < 1 ) {
			pitch = Mathf.Lerp( 2f, 1f, audioTrigger.transform.localScale.x );
		} else {
			float scaleCeiling = 4;
			float highScaleFraction = Mathf.Clamp( audioTrigger.transform.localScale.x, 0, 4) / scaleCeiling;

			pitch = Mathf.Lerp( 1f, .1f, highScaleFraction );
		}

		return pitch;
	}
}
