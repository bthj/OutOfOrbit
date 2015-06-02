using UnityEngine;
using System.Collections;

public class SubjectPlanetControl : MonoBehaviour {

	public float fadeDuration = 5f;

	private float fadeStartTime;

	private bool surfaceIntoOpaque = false;
	private Animator planetAnimator;
	private HashIDs hash;
	private float startingCutoff;

	void Start() {

		planetAnimator = gameObject.GetComponent<Animator>();
		hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();
	}
	
	// Update is called once per frame
	void Update () {
	
		if( surfaceIntoOpaque ) {

			FadeToOpaque();
		}
	}


	public void FinishSurface() {

		surfaceIntoOpaque = true;
		startingCutoff = GetComponent<Renderer>().material.GetFloat("_Cutoff");
		fadeStartTime = Time.time;
	}

	public void ShootIntoOrbit() {

		planetAnimator.SetBool( hash.intoOrbit, true );
	}


	private void FadeToOpaque() {

		float t = ( Time.time - fadeStartTime ) / fadeDuration;
		float cutoff = Mathf.SmoothStep( startingCutoff, 0f, t );

		GetComponent<Renderer>().material.SetFloat( "_Cutoff", cutoff );
	}
}
