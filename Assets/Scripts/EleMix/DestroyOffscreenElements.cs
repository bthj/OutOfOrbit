using UnityEngine;
using System.Collections;

public class DestroyOffscreenElements : MonoBehaviour {

	private ContractWorkProgression contractWorkProgression;

	void Start() {

		contractWorkProgression = 
			GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<ContractWorkProgression>();
	}

	void OnTriggerExit( Collider other ) {

		if( other.gameObject.layer != 9 ) {

			/* threshold is 3 because we still have the element that is about to be destroyed
			 * and the receiving element ring, which has the same tag.
			 */ 
			contractWorkProgression.QuitPlayIfElementTagCountIsBelowThreshold( other.gameObject.tag, 3 );

			Destroy( other.gameObject );
		}
	}
}
