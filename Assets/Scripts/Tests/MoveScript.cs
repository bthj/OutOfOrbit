using UnityEngine;
using System.Collections;

public class MoveScript : MonoBehaviour {

	// Update is called once per frame
	void Update () {
	
		// from http://unity3d.com/learn/tutorials/modules/beginner/live-training-archive/mobile-development

		if( Input.touchCount == 1) {
			/*
			transform.Translate(Input.touches[0].deltaPosition.x * .25f,
			                    Input.touches[0].deltaPosition.y * .25f,
			                    0 );
			
			*/
			Touch touch = Input.GetTouch(0);

			float x = -12f + 24 * touch.position.x / Screen.width;
			float y = -17f + 34 * touch.position.y / Screen.height;

			transform.position = new Vector3(x, y, 0);

		}
	}

	void OnGUI() {

		foreach( Touch touch in Input.touches) {

			string message = "";
			message += "ID: " + touch.fingerId + "\n";
			message += "Phase: " + touch.phase.ToString() + "\n";
			message += "TapCount: " + touch.tapCount + "\n";
			message += "Pos X: " + touch.position.x + "\n";
			message += "Pos Y: " + touch.position.y + "\n";

			int num = touch.fingerId;
			GUI.Label(new Rect(0 + 130 * num, 0, 120, 100), message);
		}
	}
}
