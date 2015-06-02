using UnityEngine;
using System.Collections;

public class ElemixAudio : MonoBehaviour {

	public AudioClip[] elementReceivedSound;
	public AudioClip elementCollidedSound;
	


	public void ElementReceived() {

		int soundIndex = Random.Range(0, elementReceivedSound.Length - 1);

		GetComponent<AudioSource>().PlayOneShot( elementReceivedSound[ soundIndex ], .1f );
	}

	public void ElementCollided() {

		if( ! GetComponent<AudioSource>().isPlaying ) {

			GetComponent<AudioSource>().PlayOneShot( elementCollidedSound, .7f );
		}
	}
}
