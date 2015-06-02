using UnityEngine;
using System.Collections;

public class ShopElementSelection : TouchBehaviour {

	/**
	 * TODO: We might want to have something general for shop items, whether
	 * they will be elements or some other itmes; for now this specific implementation
	 * for elements selection...
	 */

	//public ShopItems shopItemType;
	public Elements elementType;

	public Transform buyerDestination;  // to where bought elements animate
	public float buyTweenTime; // how long the buy animation takes
	
	public float squashRatio;  // for when we spit out elements to the buyer
	public float squashTime;
	public float stretchRatio; // for when elements bounce back from an unsuccessful purchase
	public float stretchTime;
	public float settleTime;

	private Vector3 atRestScale;

	new void Awake() {

		atRestScale = gameObject.transform.localScale;
	}


	public override void OnTouchEnd(TouchController tc, int touchIndex, Vector2 position) {

		Ray r = Camera.main.ScreenPointToRay( position );
		RaycastHit hit;
		if( Physics.Raycast(r, out hit, tc.mask ) ) {

			BuyElement();
		}
	}


	public void BuyElement() {

		bool successfulPurchase = GameStatus.instance.retail.BuyOneElementUnit(elementType);

		hatchElement( ! successfulPurchase );
	}


	private void hatchElement( bool doReject ) {

		Vector3 destinationScale = atRestScale * squashRatio;

		Hashtable squashAnimOptions = new Hashtable();
		squashAnimOptions.Add("scale", destinationScale);
		squashAnimOptions.Add("time", squashTime);
		squashAnimOptions.Add("easetype", iTween.EaseType.easeOutQuint);

		GameObject elementClone = 
			Instantiate( gameObject, gameObject.transform.position, gameObject.transform.rotation ) as GameObject;
        elementClone.layer = 2;
		elementClone.transform.localScale = elementClone.transform.localScale * squashRatio;
		Destroy( elementClone, 3 * buyTweenTime );
		
		Hashtable buyAnimOptions = new Hashtable();
		buyAnimOptions.Add( "position", buyerDestination.position );
		buyAnimOptions.Add( "time", buyTweenTime );
		//buyAnimOptions.Add( "delay", squashTime * 0 );
		buyAnimOptions.Add( "easetype", iTween.EaseType.easeInOutQuint );
		if( doReject ) {
			buyAnimOptions.Add( "oncomplete", "RejectElement" );
			buyAnimOptions.Add( "oncompletetarget", gameObject );
			buyAnimOptions.Add( "oncompleteparams", elementClone );
		} else {
			squashAnimOptions.Add("oncomplete", "SettleElement");
			squashAnimOptions.Add("oncompletetarget", gameObject);
		}
		
		//iTween.ScaleTo(gameObject, squashAnimOptions);
		
		iTween.MoveTo(elementClone, buyAnimOptions);
	}

	private void SettleElement() {

		iTween.ScaleTo( gameObject, atRestScale, settleTime );
	}

	private void RejectElement( object elementClone ) {

		Vector3 destinationScale = atRestScale * stretchRatio;

		Hashtable stretchAnimOptions = new Hashtable();
		stretchAnimOptions.Add("scale", destinationScale);
		stretchAnimOptions.Add("time", stretchTime);
		stretchAnimOptions.Add("oncomplete", "SettleElement");
		stretchAnimOptions.Add("oncompletetarget", gameObject);
		stretchAnimOptions.Add("easetype", iTween.EaseType.easeOutQuint);
		//stretchAnimOptions.Add( "delay", squashTime * .3f );

		Hashtable rejectAnimOptions = new Hashtable();
		rejectAnimOptions.Add( "position", gameObject.transform.position );
		rejectAnimOptions.Add( "time", buyTweenTime );
		rejectAnimOptions.Add( "easetype", iTween.EaseType.easeOutQuint );

		//iTween.ScaleTo(gameObject, stretchAnimOptions);
		iTween.MoveTo((GameObject)elementClone, rejectAnimOptions);
	}
	
}
