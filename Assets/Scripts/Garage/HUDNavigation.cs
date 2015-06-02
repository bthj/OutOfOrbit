using UnityEngine;
using System.Collections;

public class HUDNavigation : TouchBehaviour {

	public GarageDestinations destinationName;
	public Transform destination;
	public Transform intermediate;

	public GameObject container;
	public GameObject hud;

	public Light diodeLight;

	public float moveTweenTime = 2f;
	// if we finish the rotation tween sooner, 
	// so we'll be able to rotate by touch before the translation tween has fully smoothed in
	public float rotateTweenTime = 1f;

	private HashIDs hash;
	private DestinationBroadcast destinationBroadcast;
	private Animator hudAnimator;


	void Start () {
		
		hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();
		hudAnimator = hud.GetComponent<Animator>();

		// listen to destination events
		destinationBroadcast = 
			GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<DestinationBroadcast>();
		destinationBroadcast.destinationUpdate += 
			new DestinationBroadcast.DestinationUpdateHandler( HandleDestinationUpdate );
	}

	public override void OnTouchEnd(TouchController tc, int touchIndex, Vector2 position) {
		Ray r = Camera.main.ScreenPointToRay(position);
		RaycastHit hit;
		if (Physics.Raycast(r, out hit, tc.mask)) {
			if (hit.collider == gameObject.GetComponent<Collider>()) {

				Navigate();
			}
		}
	}


	public void LightOff() {

		diodeLight.intensity = 0;
	}

	public void LightOn() {

		diodeLight.intensity = 8;
	}



	private void Navigate() {

		MoveToDestination();

		destinationBroadcast.BroadcastDestination( destinationName );

		if( GameStatus.instance.hasVisitedDestination(destinationName) ) {
			hudAnimator.SetBool( hash.isHudUp, false );
		}
	}

	private void MoveToIntermediate() {

		iTween.MoveTo( Camera.main.gameObject, iTween.Hash(
			"position", intermediate.position, "time", moveTweenTime, 
			"oncomplete", "MoveToDestination", "oncompletetarget", gameObject) );
		iTween.RotateTo( Camera.main.gameObject, iTween.Hash(
			"rotation", intermediate.rotation.eulerAngles, "time", rotateTweenTime) );
	}

	private void MoveToDestination() {

		iTween.MoveTo( Camera.main.gameObject, iTween.Hash(
			"position", destination.position, "time", moveTweenTime) );	
		iTween.RotateTo( Camera.main.gameObject, iTween.Hash(
			"rotation", destination.rotation.eulerAngles, "time", rotateTweenTime) );
	}

	private bool IsCameraAtIntermediate() {

		return Camera.main.gameObject.transform.position == intermediate.position;
	}

	private void TurnAllNavigationLightsOff() {

		foreach( HUDNavigation oneNavInstance in container.GetComponentsInChildren<HUDNavigation>() ) {
			
			oneNavInstance.LightOff();
		}
	}

	// respond to destination events
	private void HandleDestinationUpdate( GarageDestinations broadcastDestination ) {

		hudAnimator.SetBool( hash.isHudUp, false );

		if( broadcastDestination == destinationName ) {

			TurnAllNavigationLightsOff();

			LightOn();
		}
	}
}
