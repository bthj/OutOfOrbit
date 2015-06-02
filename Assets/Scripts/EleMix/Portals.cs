using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Portals : MonoBehaviour {

	public PortalHole[] portalHoles;

	// Use this for initialization
	void Start () {

		Vector3 screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);

		Dictionary<string, Vector3> positions = new Dictionary<string, Vector3>();
		Dictionary<string, float> exitAngles = new Dictionary<string, float>();

		for( int i=0; i < portalHoles.Length; i++ ) {

			float x = Screen.width * portalHoles[i].position.x;
			float y = Screen.height * portalHoles[i].position.y;
			Vector3 portalPosition = Camera.main.ScreenToWorldPoint( new Vector3(x, y, screenPoint.z) );

			positions.Add( portalHoles[i].portalId, portalPosition );
			exitAngles.Add( portalHoles[i].portalId, portalHoles[i].exitAngle );
		}

		for( int i=0; i < portalHoles.Length; i++ ) {

			PortalHole oneHole = portalHoles[i];

			GameObject portalInstance = 
				Instantiate( oneHole.portal, positions[oneHole.portalId], oneHole.portal.transform.rotation ) as GameObject;

			Portal portalScript = portalInstance.GetComponent<Portal>();
			portalScript.elementsOnly = oneHole.elementsOnly;
			portalScript.portalId = oneHole.portalId;
			portalScript.minScale = oneHole.minScale;
			portalScript.maxScale = oneHole.maxScale;
			portalScript.scaleWaveTime = oneHole.scaleWaveTime;
			portalScript.rotationSpeed = oneHole.rotationSpeed;
			if( string.IsNullOrEmpty( oneHole.exitId ) ) {
				portalScript.exitPosition = Vector3.zero;
			} else {
				portalScript.exitPosition = positions[oneHole.exitId];
				portalScript.exitAngle = exitAngles[oneHole.exitId];
			}
			portalScript.enabled = true;
		}
	}


	[System.Serializable]
	public class PortalHole {

		public GameObject portal;
		public bool elementsOnly;
		public Vector2 position; // values in the range [0..1]
		public string portalId;
		public float exitAngle;
		public string exitId;  // if not defined, acts as a black hole, rather than a portal

		public float minScale;
		public float maxScale;
		public float scaleWaveTime; // time it takes to go from size min to max and back to min

		public int rotationSpeed;
	}
}
