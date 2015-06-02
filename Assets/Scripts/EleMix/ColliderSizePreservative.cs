using UnityEngine;
using System.Collections;

public class ColliderSizePreservative : MonoBehaviour {

	private float initialSphereColliderRadius;

	// Use this for initialization
	void Start () {
	
		SphereCollider sphereCollider =  gameObject.GetComponent<Collider>() as SphereCollider;
		if( sphereCollider != null ) {

			initialSphereColliderRadius =  sphereCollider.radius;
		}
	}
	
	public void ResetSphereColliderRadiusToInitial() {

		SphereCollider sphereCollider =  gameObject.GetComponent<Collider>() as SphereCollider;
		if( sphereCollider != null ) {

			sphereCollider.radius = 
				initialSphereColliderRadius + (sphereCollider.radius * (1-gameObject.transform.localScale.x));
		}
	}
}
