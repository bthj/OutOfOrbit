using UnityEngine;
using System.Collections;

/*
public class SwipeToCreateElement : TouchBehaviour {
*/
public class SwipeToCreateElement : MonoBehaviour {

	public GameObject fireElement;
	public GameObject waterElement;
	public GameObject airElement;
	public GameObject earthElement;

	public float creatingMaxDragFrag = 0.5f;
	public float movingMaxDragFrag = 1.0f;

	private GameObject[] currentElementClone;
	private Vector3[] velocityVector;
	private Vector3[] initialMousePos;
	private Vector3[] dragStartScale;
	private float[] dragStartTime;

	private Vector3 screenPoint;

	private ContractWorkProgression contractWorkProgression;

	private bool elementInstantiationEnabled;
	public bool ElementInstantiationEnabled {
		set {
			elementInstantiationEnabled = value;
		}
		get {
			return elementInstantiationEnabled;
		}
	}


	#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER || (!UNITY_IPHONE && !UNITY_ANDROID)
	private bool isOnTouchDevice = false;
	#else
	private bool isOnTouchDevice = true;
	#endif


/*
	public override void Awake() {

		base.Awake();
*/
	public void Awake() {

		contractWorkProgression = gameObject.GetComponent<ContractWorkProgression>();
	}
	// Use this for initialization
	void Start () {

		screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);

		currentElementClone = new GameObject[11];
		velocityVector = new Vector3[11];
		initialMousePos = new Vector3[11];
		dragStartTime = new float[11];
		dragStartScale = new Vector3[11];
		for( int i=0; i<11; i++ ) {
			velocityVector[i] = Vector3.zero;
			currentElementClone[i] = null;
		}

		elementInstantiationEnabled = false;
	}

	// Update is called once per frame
	void Update () {

		if( isOnTouchDevice ) {
			TouchInteraction();
		} else {
			MouseInteraction();
		}
	}


	private void MouseInteraction() {
		
		if( Input.GetMouseButtonDown(0) ) {

			PointingDeviceBegan( 10, Input.mousePosition );
		}
		if( Input.GetMouseButton(0) ) {

			PointingDeviceMoved( 10, Input.mousePosition );
		}

		if( Input.GetMouseButtonUp(0) ) {

			PointingDeviceEnded( 10, Input.mousePosition );
		}
	}
	
	private void TouchInteraction() { 
		
		foreach( Touch touch in Input.touches ) {
			if( touch.fingerId < 10 ) {

				switch( touch.phase ) {
				case TouchPhase.Began:
					PointingDeviceBegan( touch.fingerId, touch.position );
					break;
				case TouchPhase.Moved:
					PointingDeviceMoved( touch.fingerId, touch.position );
					break;
				case TouchPhase.Ended:
					PointingDeviceEnded( touch.fingerId, touch.position );
					break;
				default:
					break;
				}
			}
		}
	}



/*
 * Attempt at using TouchBehaviour ... it's a little more complex, as TouchBehaviour assumes
 * the script is running on the the object that received the hit, rather than a centralized class
 * such as this one, that spawns and controls objects other than containing this script...
 * ...probably I'll into a couple of classes running on their destination objects...
 * 
	public override void OnTouchStart( TouchController tc, int index, Vector3 position ) {

		velocityVector[index] = Vector3.zero;
		dragStartTime[index] = Time.time;
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay( position );
		if( Physics.Raycast(ray, out hit) ) {
			
			currentElementClone[index] = InstantiateOrGetElementByTag( hit.transform.gameObject, position );
			
			if( null != currentElementClone[index] ) {
				
				initialMousePos[index] = GetPointingPositionNormalized( position );
				dragStartScale[index] = currentElementClone[index].transform.localScale;
				
				if( Vector3.zero == currentElementClone[index].rigidbody.velocity ) {
					
					TranslateToPointingPos( currentElementClone[index].transform, position );
				}
			}
		}
	}

	public override void OnTouchDrag( TouchController tc, int index, Vector3 position ) {

		float dragAsFractionOfScreen = GetDragDistanceNormalizedAndClamped( index, position );
		
		if( null != currentElementClone[index] ) {
			
			if( currentElementClone[index].GetComponent<AddForceFromVelocity>().isOldHit ) {
				// we're touching an object that was created before
				ScaleObjectToDragDistance( currentElementClone[index], dragStartScale[index], 
				                          dragAsFractionOfScreen, movingMaxDragFrag, false );
			} else {
				// our touch just created that element clone
				TranslateToPointingPos( currentElementClone[index].transform, position );
				
				ScaleObjectToDragDistance( currentElementClone[index], dragStartScale[index], 
				                          dragAsFractionOfScreen, creatingMaxDragFrag, false );
			}
		}
	}

	public override void OnTouchEnd( TouchController tc, int index, Vector3 position ) {

		if( null != currentElementClone[index] ) {
			
			float dragAsFractionOfScreen = GetDragDistanceNormalizedAndClamped( index, position );
			velocityVector[index] = GetVelocityAsDragLength( index, position );
			
			if( currentElementClone[index].GetComponent<AddForceFromVelocity>().isOldHit
			   && Vector3.zero != velocityVector[index]) {
				// we have an already created element object, and we actually dragged away from it,
				// rather than just tapping on it - if we would allow a zero velocity vector, 
				// it would be possible to stop moving elements by tapping on them.
				
				currentElementClone[index].GetComponent<AddForceFromVelocity>().SetVelocity(velocityVector[index]);
				currentElementClone[index].GetComponent<ColliderSizePreservative>().ResetSphereColliderRadiusToInitial();
				
				ScaleObjectToDragDistance( currentElementClone[index], dragStartScale[index], 
				                          dragAsFractionOfScreen, movingMaxDragFrag, true );
				
			} else { // we have a freshly created element object, let's give it instant velocity:
				
				currentElementClone[index].rigidbody.AddForce( velocityVector[index], ForceMode.VelocityChange );
				
				ScaleObjectToDragDistance( currentElementClone[index], dragStartScale[index], 
				                          dragAsFractionOfScreen, creatingMaxDragFrag, true );
			}
		}
		
		currentElementClone[index] = null;
	}
*/

	private void PointingDeviceBegan( int index, Vector3 position ) {

		if( elementInstantiationEnabled ) {

			velocityVector[index] = Vector3.zero;
			dragStartTime[index] = Time.time;
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay( position );
			if( Physics.Raycast(ray, out hit) ) {
				
				currentElementClone[index] = InstantiateOrGetElementByTag( hit.transform.gameObject, position );
				
				if( null != currentElementClone[index] ) {
					
					initialMousePos[index] = GetPointingPositionNormalized( position );
					dragStartScale[index] = currentElementClone[index].transform.localScale;
					
					if( Vector3.zero == currentElementClone[index].GetComponent<Rigidbody>().velocity ) {
						
						TranslateToPointingPos( currentElementClone[index].transform, position );
					}
				}
			}
		} else {
			// we're not offically receiving input and the intro or outro animation is playing
			// so let's just speed up that animation
			contractWorkProgression.SpeedUpAnimation();
		}
	}
	private void PointingDeviceMoved( int index, Vector3 position ) {

		float dragAsFractionOfScreen = GetDragDistanceNormalizedAndClamped( index, position );

		if( null != currentElementClone[index] ) {

			if( currentElementClone[index].GetComponent<AddForceFromVelocity>().isOldHit ) {
				// we're touching an object that was created before
				ScaleObjectToDragDistance( currentElementClone[index], dragStartScale[index], 
				                          dragAsFractionOfScreen, movingMaxDragFrag, false );
			} else {
				// our touch just created that element clone
				TranslateToPointingPos( currentElementClone[index].transform, position );
				
				ScaleObjectToDragDistance( currentElementClone[index], dragStartScale[index], 
				                          dragAsFractionOfScreen, creatingMaxDragFrag, false );
			}
		}
	}
	private void PointingDeviceEnded( int index, Vector3 position ) {

		if( null != currentElementClone[index] ) {

			float dragAsFractionOfScreen = GetDragDistanceNormalizedAndClamped( index, position );
			velocityVector[index] = GetVelocityAsDragLength( index, position );

			if( currentElementClone[index].GetComponent<AddForceFromVelocity>().isOldHit
				   && Vector3.zero != velocityVector[index]) {
				// we have an already created element object, and we actually dragged away from it,
				// rather than just tapping on it - if we would allow a zero velocity vector, 
				// it would be possible to stop moving elements by tapping on them.
				
				currentElementClone[index].GetComponent<AddForceFromVelocity>().SetVelocity(velocityVector[index]);
				currentElementClone[index].GetComponent<ColliderSizePreservative>().ResetSphereColliderRadiusToInitial();
				
				ScaleObjectToDragDistance( currentElementClone[index], dragStartScale[index], 
				                          dragAsFractionOfScreen, movingMaxDragFrag, true );

			} else { // we have a freshly created element object, let's give it instant velocity:

				currentElementClone[index].GetComponent<Rigidbody>().AddForce( velocityVector[index], ForceMode.VelocityChange );
				
				ScaleObjectToDragDistance( currentElementClone[index], dragStartScale[index], 
				                          dragAsFractionOfScreen, creatingMaxDragFrag, true );
			}
		}
		
		currentElementClone[index] = null;
	}


	private GameObject InstantiateOrGetElementByTag( GameObject hitGameObject, Vector3 position ) {
		GameObject elementClone = null;
		bool isOld = false;
		switch( hitGameObject.tag ) {
		case "PlanetFire":
			if( contractWorkProgression.HasElements( Elements.FIRE ) ) {
				elementClone = 
					Instantiate( fireElement, GetPointingPositionInWorld(position), Quaternion.identity ) as GameObject;
				contractWorkProgression.UseOneElementUnit( Elements.FIRE );
			}
			break;
		case "PlanetWater":
			if( contractWorkProgression.HasElements( Elements.WATER ) ) {
				elementClone = 
					Instantiate( waterElement, GetPointingPositionInWorld(position), Quaternion.identity ) as GameObject;
				contractWorkProgression.UseOneElementUnit( Elements.WATER );
			}
			break;
		case "PlanetAir":
			if( contractWorkProgression.HasElements( Elements.AIR ) ) {
				elementClone = 
					Instantiate( airElement, GetPointingPositionInWorld(position), Quaternion.identity ) as GameObject;
				contractWorkProgression.UseOneElementUnit( Elements.AIR );
			}
			break;
		case "PlanetEarth":
			if( contractWorkProgression.HasElements( Elements.EARTH ) ) {
				elementClone = 
					Instantiate( earthElement, GetPointingPositionInWorld(position), Quaternion.identity ) as GameObject;
				contractWorkProgression.UseOneElementUnit( Elements.EARTH );				
			}
			break;
		case "ElementAir":
		case "ElementEarth":
		case "ElementFire":
		case "ElementWater":
			elementClone = hitGameObject;
			isOld = true;
			break;
		default:
			elementClone = null;
			break;
		}

		if( null != elementClone ) {
			AddForceFromVelocity elementForceScript = elementClone.GetComponent<AddForceFromVelocity>();
			if( null != elementForceScript ) {
				elementForceScript.isOldHit = isOld;
			} else { 
				// we're using the same tag for elements and their planet destination quarter,
				// for comparison in ElementReception.cs ,
				// and now we hit the planet and don't want it ...yeah, a bit hackish...
				elementClone = null;
			}
		}
		return elementClone;
	}


	private float GetDragDistanceNormalizedAndClamped( int index, Vector3 position ) {

		// maximum drag distance parallel to one screen axis at a time is 1,
		// diagonal drag could exceed 1 and we'll clamp that to 1

		float distance = Vector3.Distance( initialMousePos[index], GetPointingPositionNormalized( position ) );

		if( distance > 1f ) {
			distance = 1f;
		}

		return distance;
	}

	private void ScaleObjectToDragDistance( GameObject draggedObject, Vector3 startScale, 
	                                       float screenDistanceFraction, float maxDragFraction,
	                                       bool destroyIfBelowZero ) {
		// dragPenaltyRelaxation = 1 gives full penalty

		float fractionOfAllowedDrag = screenDistanceFraction / maxDragFraction;
		if( fractionOfAllowedDrag > 1f ) fractionOfAllowedDrag = 1f;

		float scaleFactor = 1f - fractionOfAllowedDrag;

		draggedObject.transform.localScale = startScale * scaleFactor;

		ScaleGameobjectsTrailRenderer( draggedObject );

		if( draggedObject.transform.localScale.x <= 0.3 && destroyIfBelowZero ) {

			/* threshold is 3 because we still have the element that is about to be destroyed
			 * and the receiving element ring, which has the same tag.
			 */ 
			contractWorkProgression.QuitPlayIfElementTagCountIsBelowThreshold( draggedObject.tag, 3 );

			DestroyImmediate( draggedObject );
		}
	}

	private void ScaleGameobjectsTrailRenderer( GameObject objectWithTrail ) {

		TrailRenderer trail = objectWithTrail.GetComponent<TrailRenderer>();
		trail.startWidth = objectWithTrail.transform.localScale.x;
	}


	private Vector3 GetVelocityAsDragLength( int index, Vector3 position ) {

		// the extra "/ (Time.time / dragStartTime)" to accomodate for a long drag that took a long time
		float deltaDragTime = (Time.time - dragStartTime[index]) * 16;
		//Debug.Log( deltaDragTime );
        //Vector3 velocity = (GetPointingPositionNormalized(position) - initialMousePos[index]) / deltaDragTime;
		Vector3 velocity = ((GetPointingPositionNormalized(position) - initialMousePos[index]) / Time.deltaTime) / deltaDragTime;
		//Debug.Log( velocity );
		return velocity;
	}

	private void TranslateToPointingPos( Transform transformInstance, Vector3 position ) {

		transformInstance.position = GetPointingPositionInWorld( position );
	}

	private Vector3 GetPointingPositionInWorld( Vector3 position ) {

		Vector3 currScreenPoint = new Vector3( position.x, position.y, screenPoint.z );
		
		return Camera.main.ScreenToWorldPoint(currScreenPoint);
	}

	private Vector3 GetPointingPositionNormalized( Vector3 position ) {

		float x = position.x / Screen.width;
		float y = position.y / Screen.height;

		return new Vector3( x, screenPoint.z, y );
	}
	
}
