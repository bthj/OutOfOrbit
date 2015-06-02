using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Text;
using UnityEngine;

[XmlRoot("MetaContract")]
public class MetaContract
{
    [field: XmlIgnore]
    private static List<Elements> DEFAULT_ORDER = FillDefaultOrder();
    [XmlElement("Requirements")]
    public SerializableDictionary<Elements, float> requirements;
    [XmlElement("Results")]
    public SerializableDictionary<Elements, float> results;
    [XmlElement("StartingElements")]
    public SerializableDictionary<Elements, float> startingElements;
    [field: XmlIgnore]
    public List<Elements> order;
    [XmlElement("Order")]
    public List<Elements> Order
    {
        get { return (order != null && order.Distinct<Elements>().Count<Elements>() < 4) ? DEFAULT_ORDER : order; }
        set { order = object.Equals(value,null)? DEFAULT_ORDER : value; }
    }
    [XmlElement("Obstacles")]
    public List<Obstacles> _obstacles;
    [property: XmlIgnore]
    public List<Obstacles> Obstacles
    {
        get { return _obstacles != null ? _obstacles.Distinct<Obstacles>().ToList<Obstacles>() : new List<Obstacles>(); }
    }
    [XmlElement("TimeLimit")]
    public int timeLimit;
    [XmlElement("TimeSpent")]
    public float timeSpent;
    [XmlElement("ISAReward")]
    public int isaReward;
    [XmlElement("TimeBonus")]
    public float timeBonus;
    [XmlElement("TimePenalty")]
    public float timePenalty;
    [XmlElement("RequiredShopItems")]
    public Retail.ShopItem[] requiredShopItems;
    [XmlElement("ContractState")]
    public ContractState state;
    [XmlElement("ContractName")]
    public string contractName; //name of the contract
    [XmlElement("ClientName")]
    public string client; //name of the client
    [XmlElement("Description")]
    public string flavourText; //description or client comment 
    [XmlElement("ObstacleMsg")]
    public string obstacles; //warning message for the player
    [XmlElement("SuccessMsg")]
    public string successMessage; // what you're greeted with after successful completion of this contract.
    [XmlElement("FailMsg")]
    public string failureMessage; // customers' sentiment in disappointment
    [field: XmlIgnore]
    public Texture2D customerImage;
    [field: XmlIgnore]
    public Texture2D finishedPlanetTexture;
    [field: XmlIgnore]
    public GameObject level;

    public MetaContract()
    {
        _obstacles = new List<Obstacles>();
    }

    private static List<Elements> FillDefaultOrder()
    {
        List <Elements> result = new List<Elements>();
        result.Add(Elements.EARTH);
        result.Add(Elements.FIRE);
        result.Add(Elements.WATER);
        result.Add(Elements.AIR);
        return result;
    }

    public static MetaContract FromContract(Contract c)
    {
        MetaContract result = new MetaContract();
        result._obstacles = c._obstacles;
        result.client = c.client;
        result.contractName = c.contractName;
        result.customerImage = c.customerImage;
        result.failureMessage = c.failureMessage;
        result.finishedPlanetTexture = c.finishedPlanetTexture;
        result.flavourText = c.flavourText;
        result.isaReward = c.isaReward;
        result.level = c.level;
        result.order = c.Order;
        result.requiredShopItems = c.requiredShopItems;
        result.requirements = c.requirements;
        result.results = c.results;
        result.startingElements = c.startingElements;
        result.successMessage = c.successMessage;
        result.timeBonus = c.timeBonus;
        result.timeLimit = c.timeLimit;
        result.timePenalty = c.timePenalty;
        result.timeSpent = c.timeSpent;
        result.state = ContractState.ACCPETED;
        return result;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is MetaContract))
            return false;
        MetaContract mc = (MetaContract) obj;
        bool result = client == mc.client;
        if (!result)
            return false;
        result = result && mc.contractName.Equals(contractName);
        if (!result)
            return false;
        result = result && mc.failureMessage.Equals(failureMessage);
        if (!result)
            return false;
        result = result && mc.isaReward.Equals(isaReward);
        if (!result)
            return false;
        result = result && CompareList(mc._obstacles,_obstacles);
        if (!result)
            return false;
        result = result && CompareList<Elements>(mc.Order,Order);
        if (!result)
            return false;
        /*result = result && CompareDictionary<Elements, float>(mc.requirements, requirements,false);
        if (mc.contractName.Equals("Flood myth") && contractName.Equals("Flood myth"))
            //Debug.LogError(mc.requirements.Count + " " + requirements.Count);
            CompareDictionary<Elements, float>(mc.requirements,requirements,true);
        if (!result)
            return false;
        result = result && mc.successMessage.Equals(successMessage);
        if (!result)
            return false;
        result = result && mc.timeBonus.Equals(timeBonus);
        if (!result)
            return false;
        result = result && mc.timeLimit.Equals(timeLimit);
        if (!result)
            return false;
        result = result && mc.timePenalty.Equals(timePenalty);
        if (!result)
            return false;*/
        return result;
    }

    private static bool CompareDictionary<Key, Value>(SerializableDictionary<Key, Value> d1, SerializableDictionary<Key,Value> d2,bool test)
    {
        if (d1.Count != d2.Count)
            return false;
        else
        {
            bool same = true;
            foreach (KeyValuePair<Key, Value> pair in d1)
            {
                same = same && d2.Contains(pair);
                if (test && !d2.Contains(pair))
                {
                    Debug.LogError(pair.Key.ToString() + " " + d1[pair.Key]+" "+d2[pair.Key]);
                }
            }
            return same;
        }
            
    }

    private static bool CompareList<T>(List<T> l1,List<T> l2)
    {
        if (l1.Count != l2.Count)
        {
            return false;
        }
        else
        {
            bool same = true;
            for (int i = 0; i < l1.Count; i++)
            {
                same = same && l1[i].Equals(l2[i]);
            }
            return same;
        }
        
    }

    public Contract FindContractInGarage()
    {

        Contract[] contracts = GameObject.Find("contracts").GetComponentsInChildren<Contract>(true);

        foreach (Contract c in contracts)
        {
            MetaContract test = MetaContract.FromContract(c);
            if (Equals(test))
            {
                return c;
            }
        }
        
        return null;
    }

    public Contract FindCurrentContractInGarage(GameStatus status)
    {
        if(status != null && status.CurrentContract != null)
            return status.CurrentContract.FindContractInGarage();
        else
            return null;
    }

    public void SetResults(float fireProgress, float earthProgress, float waterProgress, float airProgress, float workTime)
    {

        results[Elements.FIRE] = fireProgress;
        results[Elements.EARTH] = earthProgress;
        results[Elements.WATER] = waterProgress;
        results[Elements.AIR] = airProgress;

        timeSpent = workTime;
    }


	public void AddExistingElements( float fireExisting, float earthExisting, float waterExisting, float airExisting ) {
		
		results[Elements.EARTH] = earthExisting;
		results[Elements.AIR] = airExisting;
		results[Elements.FIRE] = fireExisting;
		results[Elements.WATER] = waterExisting;
	}
}
