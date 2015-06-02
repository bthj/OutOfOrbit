using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Contract : MonoBehaviour
{
    public static List<Elements> DEFAULT_ORDER = null;
	public float requiredEarth, requiredFire, requiredAir, requiredWater;
	public float existingEarth, existingFire, existingAir, existingWater;
    public SerializableDictionary<Elements, float> requirements, startingElements, results;

    public List<Elements> order;
    public List<Elements> Order
    {
		get { return object.Equals(order,null) || order.Distinct<Elements>().Count<Elements>() < 4 ? DEFAULT_ORDER : order; }
		set { order = value; }
    }
    
    public List<Obstacles> _obstacles;
    public List<Obstacles> Obstacles
    {
        get { return _obstacles == null ? new List<Obstacles>():_obstacles.Distinct<Obstacles>().ToList<Obstacles>(); }
    }
    
	public int timeLimit;
	public float timeSpent;
	public int isaReward;
	public float timeBonus;
	public float timePenalty;
	public Retail.ShopItem[] requiredShopItems;

    public string contractName; //name of the contract
	public string client; //name of the client
	public string flavourText; //description or client comment 
	public string obstacles; //warning message for the player
	public string successMessage; // what you're greeted with after successful completion of this contract.
	public string failureMessage; // customers' sentiment in disappointment
	public Texture2D customerImage;
	public Texture2D finishedPlanetTexture;
    public GameObject level;

    void Awake()
    {
        if (DEFAULT_ORDER == null)
        {
			FillDefaultOrder();
        }
        requirements = new SerializableDictionary<Elements, float>();
		requirements.Add(Elements.EARTH, requiredEarth);
		requirements.Add(Elements.AIR, requiredAir);
		requirements.Add(Elements.FIRE, requiredFire);
		requirements.Add(Elements.WATER, requiredWater);

        results = new SerializableDictionary<Elements, float>();
		results[Elements.EARTH] = existingEarth;
		results[Elements.AIR] = existingAir;
		results[Elements.FIRE] = existingFire;
		results[Elements.WATER] = existingWater;

        startingElements = new SerializableDictionary<Elements, float>();
        startingElements[Elements.EARTH] = existingEarth;
        startingElements[Elements.AIR] = existingAir;
        startingElements[Elements.FIRE] = existingFire;
        startingElements[Elements.WATER] = existingWater;
		// UpdateOrderAccordingToRequirements();
    }

    void Update()
    {
		requirements[Elements.EARTH] = Mathf.Max(0,requiredEarth);
		requirements[Elements.FIRE] = Mathf.Max(0,requiredFire);
		requirements[Elements.AIR] = Mathf.Max(0,requiredAir);
		requirements[Elements.WATER] = Mathf.Max(0,requiredWater);

		results[Elements.EARTH] = Mathf.Max(0,existingEarth);
		results[Elements.FIRE] = Mathf.Max(0,existingFire);
		results[Elements.AIR] = Mathf.Max(0,existingAir);
		results[Elements.WATER] = Mathf.Max(0,existingWater);

		// TODO: do we really have to call this here?  UpdateOrderAccordingToRequirements();
    }
	public Contract()
	{
        requirements = new SerializableDictionary<Elements, float>();
	    requirements.Add(Elements.EARTH,0);
	    requirements.Add(Elements.AIR, 0);
	    requirements.Add(Elements.FIRE, 0);
	    requirements.Add(Elements.WATER, 0);
	    isaReward = 0;
	    timeLimit = 0;

		InitializeResultsToZero();

		FillDefaultOrder();
	}

	public Contract(float fireRequired, float earthRequired, float waterRequired, float airRequired, int isa, int time)
	{
        requirements = new SerializableDictionary<Elements, float>();
	    requirements.Add(Elements.EARTH, earthRequired);
	    requirements.Add(Elements.FIRE, fireRequired);
	    requirements.Add(Elements.AIR, airRequired);
	    requirements.Add(Elements.WATER, waterRequired);
	    isaReward = isa;
	    timeLimit = time;

		InitializeResultsToZero();

		FillDefaultOrder();
	}


	public void SetResults( float fireProgress, float earthProgress, float waterProgress, float airProgress, float workTime ) {

		results[Elements.FIRE] = fireProgress;
		results[Elements.EARTH] = earthProgress;
		results[Elements.WATER] = waterProgress;
		results[Elements.AIR] = airProgress;

		timeSpent = workTime;
	}


	public void AddExistingElements( float fireExisting, float earthExisting, float waterExisting, float airExisting ) {

		results[Elements.EARTH] = existingEarth = earthExisting;
		results[Elements.AIR] = existingAir = airExisting;
		results[Elements.FIRE] = existingFire = fireExisting;
		results[Elements.WATER] = existingWater = waterExisting;
	}



	private void InitializeResultsToZero() {

        results = new SerializableDictionary<Elements, float>();
		results.Add(Elements.EARTH,0);
		results.Add(Elements.AIR, 0);
		results.Add(Elements.FIRE, 0);
		results.Add(Elements.WATER, 0);

		timeSpent = 0;
	}

	private static void FillDefaultOrder()
	{
		DEFAULT_ORDER = new List<Elements>();
		DEFAULT_ORDER.Add(Elements.EARTH);
		DEFAULT_ORDER.Add(Elements.FIRE);
		DEFAULT_ORDER.Add(Elements.WATER);
		DEFAULT_ORDER.Add(Elements.AIR);
	}

	/* elements that are existing have higher priority, 
	 * so rank them first, according to DEFAULT_ORDER
	 * and then respect the set order for the rest fo the elements.
	 *
	 * TODO: delete this maybe, probably we can assume the existing elements
	 * will probably have their correct order set in the editor
	private void UpdateOrderAccordingToRequirements() {


	}
	*/
}
