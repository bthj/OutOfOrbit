using UnityEngine;
using System.Collections;

public class LookAround : MonoBehaviour {

	public float sensitivityX = 5.0f;
	public float sensitivityY = 5.0f;
	
	public bool invertX = false;
	public bool invertY = false;

	private TouchController touchController;


	// Use this for initialization
	void Start () {
	
		touchController = GameObject.FindGameObjectWithTag(Tags.touchController).GetComponent<TouchController>();
		touchController.DragDeltaPosUpdate += new TouchController.DeltaPositionHandler( RotateToDeltaPos );
	}


	private void RotateToDeltaPos( Vector2 deltaPos, float deltaPosTime ) {

		if( deltaPosTime != 0 ) { // on an Android device this is sometimes zero
			/* maybe it's better to a check like
			 * 		float dt = Time.deltaTime / aT.deltaTime;
			 * 		if (float.IsNaN(dt) || float.IsInfinity(dt)) ...
			 * as suggested in http://answers.unity3d.com/questions/209030/android-touch-variation-correction.html
			 */

			// based on http://wiki.unity3d.com/index.php/TouchLook
			
			float rotationZ = deltaPos.x * sensitivityX * (Time.deltaTime / deltaPosTime);
			rotationZ = invertX ? rotationZ : rotationZ * -1;
			float rotationX = deltaPos.y * sensitivityY * (Time.deltaTime / deltaPosTime);
			rotationX = invertY ? rotationX : rotationX * -1;
			
			transform.localEulerAngles += new Vector3(-rotationX, rotationZ, 0);
		}
	}
}
