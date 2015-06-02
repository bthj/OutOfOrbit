using UnityEngine;
using System.Collections;

public class Retail {

	private readonly int fireBasePrice, earthBasePrice, waterBasePrice, airBasePrice;
	private readonly float priceInflationPercentage = .1f;

	public Retail( ) {

		fireBasePrice = earthBasePrice = waterBasePrice = airBasePrice = 10;
	}

	
	/** 
	 * Buys one element unit (giga ton) and 
	 * returns true if enough ISA cash is available for the purchase
	 */
	public bool BuyOneElementUnit( Elements type ) {
		bool purchaseGranted = false;
        if (GameStatus.instance.Inventory.PayCash(GetElementPrice(type)))
        {
			// we can afford the current price of this element unit price
            GameStatus.instance.Inventory.Add(type, 1);
			purchaseGranted = true;
		}
		return purchaseGranted;
	}

	/* returns the price of one element 
	 * adjusting the price according to demand
	 */
	public int GetElementPrice( Elements type ) {
        float price = GetElementBasePrice(type) * Mathf.Pow(1+priceInflationPercentage, GameStatus.instance.Inventory.GetElementAmount(type));
		return (int)price;
	}



	private int GetElementBasePrice( Elements type ) {
		int price = 0;
		switch (type){
		case Elements.EARTH:
			price = earthBasePrice;
			break;
		case Elements.FIRE:
			price = fireBasePrice;
			break;
		case Elements.WATER:
			price = waterBasePrice;
			break;
		case Elements.AIR:
			price = airBasePrice;
			break;
		}
		return price;
	}


	[System.Serializable]
	public class ShopItem {

		public ShopItems item;
		public float price;
	}
}
