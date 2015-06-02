using UnityEngine;
using System.Collections;
using System.Reflection;

public class HUDSwitch : TouchBehaviour {

	public GameObject hud;
	public float focusTime = 1f;

	private Animator hudAnimator;
	private HashIDs hash;
	private TouchController touchController;

	private Component dofComponent;
	private FieldInfo focalLengthField;

	private HUDController hudController;


	// Use this for initialization
	void Start () {
	
		hudController = gameObject.GetComponent<HUDController>();

		hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();
		hudAnimator = hud.GetComponent<Animator>();

		// let's listen to the event of a touch/click not hitting anything on the touch layer
		touchController = GameObject.FindGameObjectWithTag(Tags.touchController).GetComponent<TouchController>();
		touchController.NoTouchHit += new TouchController.NoTouchHitHandler( CloseHUDOnNoTouchHit );

		dofComponent = Camera.main.GetComponent("DepthOfFieldScatter");
		focalLengthField = dofComponent.GetType().GetField("focalLength");
	}

	public override void OnTouchEnd(TouchController tc, int touchIndex, Vector2 position) {
		Debug.Log( "OnTouchEnd" );
		Ray r = Camera.main.ScreenPointToRay(position);
		RaycastHit hit;
		if( Physics.Raycast(r, out hit, tc.mask) ) {
			if (hit.collider == gameObject.GetComponent<Collider>()) {

				ToggleHudUp();
			}
		}
	}


	public void ToggleHudUp() {

		if( hudAnimator.GetBool(hash.isHudUp) ) {
			MinimizeHud();
		} else {
			MaximizeHud();
		}

		/// we might have some info messages on the HUD, let's clear them 
		/// and bring back the current contract on a plain on' toggle.
		hudController.RenderCurrentContract();
		hudController.RenderContractStamps();
	}

	public void MinimizeHud() {

		hudAnimator.SetBool( hash.isHudUp, false );
	
		StartCoroutine( FocusPuller(.3f, .5f) );
	}

	public void MaximizeHud() {

		hudAnimator.SetBool( "isHudUp", true );

		StartCoroutine( FocusPuller(.5f, .3f) );
	}


	/* ------- event listening -------- */

	private void CloseHUDOnNoTouchHit() {
		
		MinimizeHud();
	}


	IEnumerator FocusPuller( float focalLengthFrom, float focalLengthTo ) {

		float focusTimer = 0f;
		while( focusTimer < focusTime ) {

			focalLengthField.SetValue(dofComponent,  Mathf.Lerp(focalLengthFrom, focalLengthTo, focusTimer/focusTime) );

			focusTimer += Time.deltaTime;

			yield return null;
		}
	}
}
