using UnityEngine;
using System.Collections;

public class AsteroidSpawner : MonoBehaviour {
	
	public SpawnSegment[] spawnSegments;
	
	private float spawnRingRadius;

	private Vector3 stageDimensions;
	private Vector3 screenPoint;
	
	void Start () {

		screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);

		stageDimensions = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, screenPoint.z ));
	
		// Pythagorean...
		spawnRingRadius = Mathf.Sqrt( Mathf.Pow(stageDimensions.x, 2) + Mathf.Pow(stageDimensions.z, 2) );

		for( int i=0; i < spawnSegments.Length; i++ ) {

			StartCoroutine( SpawnWaves(i) );
		}
	}

	void OnDestroy() {

		StopAllCoroutines();  // maybe not neccessary?
	}

	IEnumerator SpawnWaves( int segmentIndex ) {
		SpawnSegment segment = spawnSegments[segmentIndex];

		yield return new WaitForSeconds( segment.startWait );

		while( true ) {
			int asteroidCountInWave = Random.Range(segment.spawnCountFrom, segment.spawnCountTo);
			for( int i=0; i < asteroidCountInWave; i++ ) {

				float spawnRingPosition = Random.Range(segment.degreeFrom, segment.degreeTo);
				float x = Mathf.Cos( spawnRingPosition * Mathf.Deg2Rad ) * spawnRingRadius;
				float z = - Mathf.Sin( spawnRingPosition * Mathf.Deg2Rad ) * spawnRingRadius;
				Vector3 spawnPosition = new Vector3( x, 0, z );
				
				GameObject asteroidInstance = 
					Instantiate( segment.asteroid, spawnPosition, Quaternion.identity ) as GameObject;

				asteroidInstance.transform.localScale *= segment.sizePercent;
				
				// set random rotation on the asteroid
				asteroidInstance.GetComponent<Rigidbody>().angularVelocity = Random.insideUnitSphere * segment.tumble;
				// and move it
				asteroidInstance.GetComponent<Rigidbody>().velocity = 
					Quaternion.AngleAxis(segment.moveDirectionDeg, Vector3.up) 
						* Vector3.one * segment.speed;

				yield return new WaitForSeconds( Random.Range(segment.spawnWaitFrom, segment.spawnWaitTo) );
			}
			yield return new WaitForSeconds( Random.Range(segment.waveWaitFrom, segment.waveWaitTo) );
		}
	}

	[System.Serializable]
	public class SpawnSegment {

		public GameObject asteroid;
		public float sizePercent = 1f;
		public float startWait;
		public float speed;
		public float tumble;

		// in what direction, in degrees, to shoot the asteroids
		public float moveDirectionDeg;
		// degrees start counting in the East, along the positive x axis
		public float degreeFrom, degreeTo;
		// range to pick a random value from for one wave spawn count
		public int spawnCountFrom, spawnCountTo;
		// range to pick a random waiting time from, between individual spawns
		public float spawnWaitFrom, spawnWaitTo;
		// range to pick random waiting time from, between spawn waves
		public float waveWaitFrom, waveWaitTo;
	}
}
