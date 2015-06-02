using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public GameObject cloud;
	public GameObject drain;
	public GameObject drop;

	public int cloudRows;
	public int cloudsPerRow;
	public Vector3 cloudSpawnValues;

	public int dropsPerCircle;
	public int circlesPerCloud;
	public int dropRndHorizOffset;
	public int dropRndVertOffset;

	private float radiansPerDrop;
	private float cloudHalfSize;
	private float dropCircleYSpace;

	// Use this for initialization
	void Start () {
		radiansPerDrop = (2 * Mathf.PI) / dropsPerCircle;
		cloudHalfSize = cloud.GetComponent<Renderer>().bounds.size.x / 2;
		dropCircleYSpace = cloudSpawnValues.y / circlesPerCloud;

		InstantiateClouds();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate() {
/*
		foreach( GameObject oneDrop in GameObject.FindGameObjectsWithTag("Drop") ) {

			oneDrop.rigidbody.AddForce( new Vector3(0, -0.1f, 0) );
		}
*/
	}

	void InstantiateClouds() {
		for( int rowCount = 0; rowCount < cloudRows; rowCount++ ) {
			for( int rowCloudCount = 0; rowCloudCount < cloudsPerRow; rowCloudCount++ ) {
				float startZ = rowCount * cloudSpawnValues.z;
				Vector3 cloudSpawnPosition = 
					new Vector3( 
			                 Random.Range(-cloudSpawnValues.x, cloudSpawnValues.x), 
			                 cloudSpawnValues.y, 
			                 Random.Range(startZ, startZ + cloudSpawnValues.z) 
			            );
				if( Physics.CheckSphere(cloudSpawnPosition, cloudHalfSize ) ) {
					// a new cloud would overlap an existing one given this position, so let's try again
					rowCloudCount--;  // infinite loop danger!  
					// ...we have to make sure there is enough room in cloudSpawnValues for all clouds without overlapping.
				} else {
					Instantiate( cloud, cloudSpawnPosition, Quaternion.identity );
					// Instantiate( drain, new Vector3(cloudSpawnPosition.x, -1f, cloudSpawnPosition.z), Quaternion.identity );
					InstantiateDrops( cloudSpawnPosition );
				}
			}
		}
	}

	void InstantiateRingOfDropsUnderCloud( Vector3 center ) {
		for( int dropCount = 0; dropCount < dropsPerCircle; dropCount++ ) {

			float dropXPos = Mathf.Cos(dropCount * radiansPerDrop) * (cloudHalfSize-dropRndHorizOffset) + center.x;
			float dropZPos = Mathf.Sin(dropCount * radiansPerDrop) * (cloudHalfSize-dropRndHorizOffset) + center.z;
			Vector3 dropSpawnPosition = 
				new Vector3(
							Random.Range(dropXPos - dropRndHorizOffset, dropXPos + dropRndHorizOffset),
							Random.Range( center.y - dropRndVertOffset, center.y + dropRndVertOffset ),
				            Random.Range(dropZPos - dropRndHorizOffset, dropZPos + dropRndHorizOffset)
					);
			Instantiate( drop, dropSpawnPosition, Quaternion.identity );
		}	
	}

	void InstantiateDrops( Vector3 center ) {
		float initialInstantiationY = center.y;

		for( int circleCount = 0; circleCount < circlesPerCloud; circleCount++ ) {
			initialInstantiationY -= dropCircleYSpace;

			InstantiateRingOfDropsUnderCloud( new Vector3(center.x, initialInstantiationY, center.z) );
		}

		StartCoroutine( SpawnOneRingOfDrops(center) );
	}

	IEnumerator SpawnOneRingOfDrops( Vector3 center ) {

		while( true ) {
			yield return new WaitForSeconds( 2f );

			InstantiateRingOfDropsUnderCloud( center );
		}

	}
}
