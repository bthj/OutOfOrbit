using UnityEngine;
using System.Collections;

public class DropFall : MonoBehaviour {

	public float fallSpeed;

	private Vector3 dropStart;
	private Vector3 dropDestination;
	private float fallDistance;
	private float distanceCovered;

	// Use this for initialization
	void Start () {
	
		dropStart = transform.position;
		dropDestination = new Vector3( transform.position.x, -1f, transform.position.z );
		fallDistance = Vector3.Distance( dropStart, dropDestination );
	}

	void Update () {

		distanceCovered += Time.deltaTime * fallSpeed;

		float fractionOfDistance = distanceCovered / fallDistance;

		if( fractionOfDistance >= 1 ) {

			Destroy( gameObject );
		} else {


			transform.position = Vector3.Lerp( dropStart, dropDestination, fractionOfDistance );
		}
	}
}
