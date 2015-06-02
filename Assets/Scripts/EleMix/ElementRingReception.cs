using UnityEngine;
using System.Collections;

public class ElementRingReception : MonoBehaviour {

	public Elements element;
	public float elementFullSize = 1f;

	private ContractWorkProgression contractWorkProgression;
	private ElemixAudio elemixAudio;

	void Start() {

		contractWorkProgression = 
			GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<ContractWorkProgression>();
		elemixAudio = GameObject.Find("AudioController").GetComponent<ElemixAudio>();

		GetComponent<Renderer>().material.SetFloat("_Blend", 0f);
	}
	
	void OnTriggerEnter( Collider other ) {
		
		if( gameObject.tag == other.gameObject.tag ) {
			
			gameObject.GetComponent<Collider>().isTrigger = true;


			float fullSizeFraction = GetElementSizeFraction( other.gameObject );

			contractWorkProgression.UpdateElementTally( element, fullSizeFraction );

			float blend = contractWorkProgression.GetElementProgressPercentage( element );
			GetComponent<Renderer>().material.SetFloat( "_Blend",  blend );

			/* threshold is 3 because we still have the element that is about to be destroyed
			 * and the receiving element ring, which has the same tag.
			 */ 
			contractWorkProgression.QuitPlayIfElementTagCountIsBelowThreshold( other.gameObject.tag, 3 );

			#if UNITY_IPHONE || UNITY_ANDROID
			Handheld.Vibrate();
			#endif

			elemixAudio.ElementReceived();

			Destroy( other.gameObject );
			
		} else {
			
			gameObject.GetComponent<Collider>().isTrigger = false;
		}
	}


	public void SetTargetTexture( Texture2D targetTexture ) {

		GetComponent<Renderer>().material.SetTexture( "_Texture2", targetTexture );
	}
	
	
	
	private float GetElementSizeFraction( GameObject element ) {
		
		return element.transform.localScale.x / elementFullSize;
	}

}
