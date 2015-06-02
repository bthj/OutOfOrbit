using UnityEngine;
using System.Collections;

public class MissionCameraAnimationEvents : MonoBehaviour {

	private SceneFadeInOut sceneFadeInOut;

	void Awake() {

		sceneFadeInOut = GameObject.FindGameObjectWithTag(Tags.fader).GetComponent<SceneFadeInOut>();
	}
	
	// Update is called once per frame
	void Update () {
	}


	public void DisbandElementRings() {

		foreach( Transform oneRing in GameObject.Find("FinishedPlanet/elementRings").transform ) {

			oneRing.GetComponent<RandomRotator>().ShootIntoRandomness();
		}
	}

	public void CoverPlanetSurface() {

		GameObject.Find( "FinishedPlanet" ).GetComponent<SubjectPlanetControl>().FinishSurface();
	}

	public void DispatchFinishedPlanet() {

		GameObject.Find( "FinishedPlanet" ).GetComponent<SubjectPlanetControl>().ShootIntoOrbit();
	}

	public void FadeOutAndExit() {

        sceneFadeInOut.EndScene();
	}
}
