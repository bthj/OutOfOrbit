using UnityEngine;
using System.Collections;

public class RandomRotator : MonoBehaviour {

	public float maxRotationSpeed = 60.0f; // max 60 degrees per second
	public Vector3 rotationAxis;
	public float shootSpeed = 1f;
	
	private Vector3 rotSpeed; // this will hold the XYZ velocities
	
	void Start(){
		// create random angular velocities around the 3 axes at once:
		//
		rotSpeed = Random.insideUnitSphere * maxRotationSpeed;
		//rotSpeed = Vector3.left * maxRotationSpeed;
		//rotSpeed = rotationAxis * maxRotationSpeed;
	}
	
	void Update() {

		if( Input.GetMouseButtonDown(0) ) {
			// let's get a new rotaton angle
			rotSpeed = Random.insideUnitSphere * maxRotationSpeed;
			// Debug.Log( "New rotatation angle: " + rotSpeed );
		}

		// rotate the object continuously:
		transform.Rotate(rotSpeed * Time.deltaTime);
	}


	public void ShootIntoRandomness() {

		GetComponent<Rigidbody>().isKinematic = false;
		GetComponent<Rigidbody>().AddForce( Random.insideUnitSphere * shootSpeed );
	}
}
