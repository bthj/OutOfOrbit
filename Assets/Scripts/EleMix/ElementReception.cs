using UnityEngine;
using System.Collections;

public class ElementReception : MonoBehaviour {
	
	public float elementFullSize = 1f;
	public int fullCoverageCount = 10;
	// public float desiredPercentage = 1f;

	private float elementCoverage = 0f;

	void Start() {

		GetComponent<Renderer>().material.SetFloat("_Blend", 0f);
	}

	void OnTriggerEnter( Collider other ) {

		if( gameObject.tag == other.gameObject.tag ) {

			gameObject.GetComponent<Collider>().isTrigger = true;

			UpdateTotalCoverageFraction(other.gameObject);

			GetComponent<Renderer>().material.SetFloat("_Blend",  elementCoverage );

			Destroy( other.gameObject );

		} else {

			gameObject.GetComponent<Collider>().isTrigger = false;
		}

	}



	private float GetElementSizeFraction( GameObject element ) {

		return element.transform.localScale.x / elementFullSize;
	}

	private float GetCoverageFractionFromElementHit( GameObject element ) {

		return GetElementSizeFraction(element) / fullCoverageCount;
	}

	private void UpdateTotalCoverageFraction( GameObject element ) {

		elementCoverage += GetCoverageFractionFromElementHit(element);
	}
}
