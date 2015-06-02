using System;
using System.Xml;
using System.Xml.Serialization;

[XmlRoot("Inventory")]
public class Inventory
{
    public int Earth { get; set; }
    public int Fire { get; set; }
    public int Water { get; set; }
    public int Air { get; set; }
    public int Cash { get; set; }

    public delegate void InventoryUpdateDelegate();
    public static event InventoryUpdateDelegate InventoryUpdate;

    public Inventory()
    {
        Earth = Fire = Water = Air = Cash = 0;
    }


	public static void OnInventoryUpdate() {
		if( GameStatus.instance != null && GameStatus.instance.Inventory != null && null != InventoryUpdate) {
			
			InventoryUpdate();
            
		}
	}


    public override string ToString()
    {
        return "Earth:" + Earth + "\nFire:" + Fire + "\nWater:" + Water + "\nAir:" + Air + "\nCash:" + Cash;
    }

    public void Add(Elements type, int amount)
    {
		bool didUpdate = false;
        switch (type)
        {
            case Elements.EARTH:
                Earth += amount;
				didUpdate = true;
                break;
            case Elements.FIRE:
                Fire += amount;
				didUpdate = true;
                break;
            case Elements.WATER:
                Water += amount;
				didUpdate = true;
                break;
            case Elements.AIR:
                Air += amount;
				didUpdate = true;
                break;
        }
		if( didUpdate ) OnInventoryUpdate();
    }

    private void Pay(Elements type, int amount)
    {
		bool didUpdate = false;
        switch (type)
        {
            case Elements.EARTH:
                Earth -= amount;
				didUpdate = true;
                break;
            case Elements.FIRE:
                Fire -= amount;
				didUpdate = true;
                break;
            case Elements.WATER:
                Water -= amount;
				didUpdate = true;
                break;
            case Elements.AIR:
                Air -= amount;
				didUpdate = true;
                break;
        }
		if( didUpdate ) OnInventoryUpdate();
    }
    public bool PayElements(int earth, int fire, int water, int air)
    {
        bool res = Earth >= earth && Fire >= fire && Water >= water && Air >= air && earth >= 0 && fire >= 0 && water >= 0 && air >= 0;
        if (res)
        {
            Pay(Elements.EARTH, earth);
            Pay(Elements.FIRE, fire);
            Pay(Elements.WATER, water);
            Pay(Elements.AIR, air);
        }
        return res;
    }
	

	public int CheckoutAllOfElement( Elements type ) {
		
		int amount = GetElementAmount( type );
		
		Pay( type, amount );
		
		return amount;
	}


    public bool PayCash(int amount)
    {
        bool res = Cash >= amount && amount >= 0;
        if (res) {
            Cash -= amount;
			OnInventoryUpdate();
		}
        return res;
    }

	public bool AddElements(int fire, int earth, int water, int air)
    {
        bool res = earth >= 0 && fire >= 0 && water >= 0 && air >= 0;
        if (res)
        {
            Add(Elements.EARTH, earth);
            Add(Elements.FIRE, fire);
            Add(Elements.WATER, water);
            Add(Elements.AIR, air);
        }
        return res;
    }

    public bool AddCash(int amount)
    {
        bool res = amount >= 0;
        if (res) {
            Cash += amount;
			OnInventoryUpdate();
		}
        return res;
    }

	public int GetElementAmount( Elements type ) {
        switch (type)
        {
            case Elements.EARTH:
                return Earth;
            case Elements.FIRE:
				return Fire;
            case Elements.WATER:
				return Water;
            case Elements.AIR:
				return Air;
			default:
                return 0;
        }
	}

}