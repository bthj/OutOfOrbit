using UnityEngine;
using System.Collections;

public class ShopLabelElement : MonoBehaviour {

	public Elements element;

	private TextMesh label;

	void Start () {

		// listen to inventory update events
		Inventory.InventoryUpdate += UpdateLabelWithPrice;

		label = gameObject.GetComponent<TextMesh>();

		UpdateLabelWithPrice();
	}
	
	private void UpdateLabelWithPrice() {

		int currentElementPrice = GameStatus.instance.retail.GetElementPrice( element );
		label.text = "1 gt. " + element + " @ " + currentElementPrice + ",- I$A";
	}

    void OnDestroy()
    {
        Inventory.InventoryUpdate -= UpdateLabelWithPrice;
    }
}
