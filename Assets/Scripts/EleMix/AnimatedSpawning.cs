using UnityEngine;
using System.Collections;

public class AnimatedSpawning : MonoBehaviour {

	public float stretchTime = 1f, settleTime = .5f; // somewhat like http://en.wikipedia.org/wiki/12_basic_principles_of_animation#Squash_and_stretch
	public Vector3 stretch;

	private Vector3 atRestScale;

	void Awake() {

		atRestScale = gameObject.transform.localScale;
		
		gameObject.transform.localScale = Vector3.zero;
	}

	public void OnEnable() {

		// NOTE: If the game object is set as active in the Unity Editor
		//	then gameObject.activeSelf will be true here on game start
		// 	even though .SetActive is set false later in script (ContractWorkProgression)
		//	-> so it has to be set as inactive in the editor.

		if( gameObject.activeSelf ) {

			// start a stretch and settle to normal size of object
			StartCoroutine( ElasticSpawn() );
		}
	}

	IEnumerator ElasticSpawn() {

		float stretchTimer = 0f;
		float settleTimer = 0f;

		Vector3 stretchScale = atRestScale + stretch;
		while( stretchTimer < stretchTime && transform.localScale.x / stretchScale.x < .95f) {

			stretchTimer += Time.deltaTime;

			// lerping _from_ current local scale creates a spring like motion
			// (moves faster the further we proceed)
			transform.localScale = Vector3.Lerp( transform.localScale, stretchScale, stretchTimer / stretchTime );

			yield return null;
		}

		while( settleTimer < settleTime ) {

			settleTimer += Time.deltaTime;

			transform.localScale = Vector3.Lerp( transform.localScale, atRestScale, settleTimer / settleTime );

			yield return null;
		}
	}
}
