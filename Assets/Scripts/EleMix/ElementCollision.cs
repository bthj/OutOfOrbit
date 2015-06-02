using UnityEngine;
using System.Collections;

public class ElementCollision : MonoBehaviour {

	private ElemixAudio elemixAudio;

	void Start() {

		elemixAudio = GameObject.Find("AudioController").GetComponent<ElemixAudio>();
	}
	

	void OnCollisionEnter( Collision other ) {

		#if UNITY_IPHONE || UNITY_ANDROID
		Handheld.Vibrate();
		#endif

		elemixAudio.ElementCollided();
	}
}
