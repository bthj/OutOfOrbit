using UnityEngine;
using System.Collections;

public class AddForceFromVelocity : MonoBehaviour {

	public bool isOldHit;  // true if object was hit after it's creation

	//private Vector3 velocityVector; // set by swiping on object after creation


	void Start() {
		//velocityVector = Vector3.zero;
	}

	void FixedUpdate() {

		/*if( Vector3.zero != velocityVector ) {

			// swipes on an already instantiated element instance

			
			rigidbody.AddForce( velocityVector * 12 * Time.fixedDeltaTime, ForceMode.Force );
		}*/
	}

	public void SetVelocity( Vector3 velocity ) {

		//velocityVector = velocity;
        /*rigidbody.velocity = Vector3.zero;
        rigidbody.AddForce(velocity,ForceMode.VelocityChange);*/
        GetComponent<Rigidbody>().velocity = velocity;
	}

	public void SubtractFromScale( float fraction ) {

	}
}
