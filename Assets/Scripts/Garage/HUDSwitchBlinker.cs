using UnityEngine;
using System.Collections;

public class HUDSwitchBlinker : MonoBehaviour {

	public bool isTypeOn;
	public GameObject hud;
	public Light diodeLight;
	public float blinkSpeed = 1f;

	private Animator hudAnimator;
	private HashIDs hash;

	// Use this for initialization
	void Start () {
	
		hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();
		hudAnimator = hud.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
	
		BlinkLight();
	}


	private void BlinkLight() {
		
		if( isTypeOn == ! hudAnimator.GetBool(hash.isHudUp) ) {
			
			float lerpFraction = Mathf.PingPong(Time.time, blinkSpeed) / blinkSpeed;
			diodeLight.intensity = Mathf.Lerp( 1, 8, lerpFraction );
			
		} else {
			
			diodeLight.intensity = 0;
		}
	}
}
