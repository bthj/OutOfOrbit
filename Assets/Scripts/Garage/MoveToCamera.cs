using UnityEngine;
using System.Collections;

public class MoveToCamera : TouchBehaviour {

	public GarageDestinations destinatioName,secondaryDestination;
	public Camera cam,secondaryCam;

	public float moveTweenTime = 2f;
	// if we finish the rotation tween sooner, 
	// so we'll be able to rotate by touch before the translation tween has fully smoothed in
	public float rotateTweenTime = 1f;

	private DestinationBroadcast destinationBroadcast;

	void Start() {

		destinationBroadcast = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<DestinationBroadcast>();
	}


	public override void OnTouchEnd(TouchController tc, int touchIndex, Vector2 position)
    {
        Ray r = Camera.main.ScreenPointToRay(position);
        RaycastHit hit;
        if (Physics.Raycast(r, out hit, tc.mask))
        {
            if (hit.collider == gameObject.GetComponent<Collider>())
            {
				NavigaetToHit();
            }
        }
    }


	private void NavigaetToHit() {

		GameObject go = new GameObject();
		go.transform.position = Camera.main.gameObject.transform.position;
		go.transform.rotation = Camera.main.gameObject.transform.rotation;
		Camera.main.gameObject.GetComponentInChildren<GoBack>().previousPositions.Push(go);
        if (go.transform.position != cam.transform.position)
        {
            iTween.MoveTo(Camera.main.gameObject, cam.transform.position, moveTweenTime);
            iTween.RotateTo(Camera.main.gameObject, cam.transform.rotation.eulerAngles, rotateTweenTime);
            destinationBroadcast.BroadcastDestination(destinatioName);
        }
        else
        {
            iTween.MoveTo(Camera.main.gameObject, secondaryCam.transform.position, moveTweenTime);
            iTween.RotateTo(Camera.main.gameObject, secondaryCam.transform.rotation.eulerAngles, rotateTweenTime);
            destinationBroadcast.BroadcastDestination(secondaryDestination);
        }
		// let's finish the rotation tween sooner, 
		// so we'll be able to rotate by touch before the translation tween has fully smoothed in
		

		
	}
}
