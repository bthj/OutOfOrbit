using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TouchController : MonoBehaviour {

    public static int TOUCH_LAYER = 8;
	// diode spheres are on their own layer for lighting and we want to be able to hit them also:
	public static int DIODE_LAYER = 11;
	public readonly int mask = (1 << TOUCH_LAYER) | (1 << DIODE_LAYER);
	// int.MaxValue should not interfere with Touch.fingerId
	public static int MOUSE_INDEX = int.MaxValue;

    public static TouchController instance;
	public Dictionary<int,TouchBehaviour> selected;
	// let's record where touches began, to be able to calculate drag distance 
	// and thus determine if we'll fire a touch ended event; 
	// if dragged long enough, we won't call it a tap but just a drag
	private Dictionary<int, Vector2> touchDownPosition;
	private float maxTapDragLength; // if we'll drag longer than this, it ain't a tap!
	private Vector3 mouseLastDragPos;  // for mouse delta drag computation, Touch provides it already!
	private float mousePosDeltaTime;


	public delegate void NoTouchHitHandler();
	public event NoTouchHitHandler NoTouchHit;

	public delegate void DeltaPositionHandler( Vector2 deltaPos, float deltaPosTime );
	public event DeltaPositionHandler DragDeltaPosUpdate;


	#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER || (!UNITY_IPHONE && !UNITY_ANDROID)
		private bool isOnTouchDevice = false;
	#else
		private bool isOnTouchDevice = true;
	#endif


    void Awake()
    {
        if (instance != null)
            Destroy(this);
        else
            instance = this;
    }
	// Use this for initialization
	void Start () {
        selected = new Dictionary<int,TouchBehaviour>();
		touchDownPosition = new Dictionary<int, Vector2>();
		mouseLastDragPos = Vector2.zero;
		mousePosDeltaTime = 0f;

		// let's allow a tap to drag < 5% of the screens diagonal
		maxTapDragLength = 
			Mathf.Sqrt( Mathf.Pow(Camera.main.pixelWidth, 2) + Mathf.Pow(Camera.main.pixelHeight, 2) ) * .04f;
	}
	
	// Update is called once per frame
	void Update () {

		if( isOnTouchDevice ) {

			TouchInteraction();
		} else {
			MouseInteraction();
		}
	}


	private void TouchInteraction() {
		foreach (Touch t in Input.touches) {
			if( t.fingerId < MOUSE_INDEX ) {

				switch (t.phase) {
				case TouchPhase.Began:
					PointingDeviceBegan( t.fingerId, t.position );
					break;
				case TouchPhase.Moved:
					PointingDeviceMoved( t.fingerId, t.position, t.deltaPosition, t.deltaTime );
					break;
				case TouchPhase.Ended:
					PointingDeviceEnded( t.fingerId, t.position );
					break;
				default:
					break;
				}
			}
		}
	}

	private void MouseInteraction() {

		if( Input.GetMouseButtonDown(0) ) {

			mouseLastDragPos = Input.mousePosition;
			
			PointingDeviceBegan( MOUSE_INDEX, Input.mousePosition );
		}
		if( Input.GetMouseButton(0) ) {

			Vector2 deltaPosition = Input.mousePosition - mouseLastDragPos;
			mousePosDeltaTime = Time.time - mousePosDeltaTime;
			
			PointingDeviceMoved( MOUSE_INDEX, Input.mousePosition, deltaPosition, mousePosDeltaTime );

			mouseLastDragPos = Input.mousePosition;
			mousePosDeltaTime = Time.time;
		}
		
		if( Input.GetMouseButtonUp(0) ) {
			
			PointingDeviceEnded( MOUSE_INDEX, Input.mousePosition );
		}
	}


	private void PointingDeviceBegan( int index, Vector2 position ) {

		Ray r = Camera.main.ScreenPointToRay( position );
		RaycastHit hit;
		if( Physics.Raycast(r, out hit, Mathf.Infinity, mask) ) {
			
			if( !selected.ContainsKey( index ) ) {
				selected.Add( index, hit.collider.gameObject.GetComponent<TouchBehaviour>() );
				touchDownPosition.Add( index, position );
			} else {
				selected[ index ] = hit.collider.gameObject.GetComponent<TouchBehaviour>();
				touchDownPosition[ index ] = position;
			}
			selected[index].OnTouchStart( this, index, position );
		} else {
			// nothing in the touch layer was hit, tell anyone interested in that event about it
			OnNoTouchHit();
		}
	}

	private void PointingDeviceMoved( int index, Vector2 position, Vector2 deltaPos, float deltaPosTime ) {

		if( selected.ContainsKey(index) && selected[index] != null ) {

			selected[index].OnTouchDrag( this, index, position );
		}

		// let's broadcast the drag delta, that for example the camera can pick up
		OnDragDeltaPosUpdate( deltaPos, deltaPosTime );
	}

	private void PointingDeviceEnded( int index, Vector2 position ) {

		if( selected.ContainsKey(index) && selected[index] != null && 
		   // a tap can only drag maxTapDragLength
		   (touchDownPosition.ContainsKey(index) && Vector2.Distance( position, touchDownPosition[index] ) < maxTapDragLength) ) {

			selected[index].OnTouchEnd( this, index, position );
		}
		selected[index] = null;
	}
	


	/* ------- event broadcast -------- */

	private void OnNoTouchHit() {
		
		if( null != NoTouchHit ) {
			
			NoTouchHit();
		}
	}

	private void OnDragDeltaPosUpdate( Vector2 deltaPos, float deltaPosTime ) {

		if( null != DragDeltaPosUpdate ) {

			DragDeltaPosUpdate( deltaPos, deltaPosTime );
		}
	}
}
