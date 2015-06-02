using UnityEngine;
using System.Collections;

public class TranslateElementPlanetsToScreenCorners : MonoBehaviour {

	public TextMesh fireStockHUD, earthStockHUD, waterStockHUD, airStockHUD;

	private Vector3 screenPoint;

	// Use this for initialization
	void Start () {

		screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
	

		Vector3 fireCorner = Camera.main.ScreenToWorldPoint( new Vector3(0f, 0f, screenPoint.z) );

		float worldSpanX = Mathf.Abs( fireCorner.x * 2 );
		float worldSpanZ = Mathf.Abs( fireCorner.z * 2 );

		GameObject firePlanet = GameObject.FindGameObjectWithTag("PlanetFire");
		if( null != firePlanet ) firePlanet.transform.position = fireCorner;
		fireCorner.z += 1;
		fireStockHUD.transform.position = fireCorner;


		Vector3 waterCorner = Camera.main.ScreenToWorldPoint( new Vector3(0f, Screen.height, screenPoint.z) );
		GameObject waterPlanet = GameObject.FindGameObjectWithTag("PlanetWater");
		if( null != waterPlanet ) waterPlanet.transform.position = waterCorner;
		waterStockHUD.transform.position = waterCorner;


		Vector3 earthCorner = Camera.main.ScreenToWorldPoint( new Vector3(Screen.width, Screen.height, screenPoint.z) );
		GameObject earthPlanet = GameObject.FindGameObjectWithTag("PlanetEarth");
		if( null != earthPlanet ) earthPlanet.transform.position = earthCorner;
		earthCorner.x -= .5f;
		earthStockHUD.transform.position = earthCorner;


		Vector3 airCorner = Camera.main.ScreenToWorldPoint( new Vector3(Screen.width, 0f, screenPoint.z) );
		GameObject airPlanet = GameObject.FindGameObjectWithTag("PlanetAir");
		if( null != airPlanet ) airPlanet.transform.position = airCorner;
		airCorner.x -= .5f;
		airCorner.z += 1;
		airStockHUD.transform.position = airCorner;



		GameObject boundary = GameObject.Find("Boundary");
		boundary.transform.localScale = new Vector3( worldSpanX + 1, boundary.transform.localScale.y, worldSpanZ + 1 );
	}
}
