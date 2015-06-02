using UnityEngine;
using System.Collections;

public class AbsorbDrop : MonoBehaviour {

	void OnTriggerEnter( Collider other ) {

		if( other.tag == "Drop" ) {

			Destroy( other.gameObject );
		}
	}
}
