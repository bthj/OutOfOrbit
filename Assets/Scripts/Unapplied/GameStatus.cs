using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

[XmlRoot("GameStatus")]
public class GameStatus
{

    [field: XmlIgnore]
    public static GameStatus instance = FromFileOrNew();
    [field: XmlIgnore]
    public Retail retail;
    
	[XmlElement("Upgrades")]
    public List<ShopItems> upgrades;

	[XmlElement("DestinationsVisited")]
	public List<GarageDestinations> destinationsVisited;

    [property: XmlElement("Inventory")]
    public Inventory Inventory { get; set; }

    [XmlElement("CompletedContracts")]
    public List<MetaContract> CompletedContracts { get; set; }

    [property: XmlIgnore]
    public MetaContract CurrentContract
    {
        set
        {
            currentContract = value;
            OnContractUpdate();  // send out an event
        }
        get
        {
            return currentContract;
        }
    }

    /* shouldn't we just store our balance ISA in Inventory?
    public int ISA {
        set {
            isa = value;
            OnISAUpdate();
        }
        get {
            return isa;
        }
    }
    */
    public delegate void GameStatusUpdateDelegate();
    public static event GameStatusUpdateDelegate ContractUpdate; //, ISAUpdate;

    [field: XmlElement("CurrentContract")]
    public MetaContract currentContract;
    //	private int isa;


    /*-- Not used --
    public bool zoomed;
    private Vector3 startpos, leftwall, rightwall, backwall;
    private Quaternion start, left, right, back;
    //--------------*/
    //private bool t = false;

    private GameStatus()
    {
        Inventory = new Inventory();
        retail = new Retail();
        CompletedContracts = new List<MetaContract>();
        
		upgrades = new List<ShopItems>();

		destinationsVisited = new List<GarageDestinations>();

        // let's give the player som initial cash
        // TODO: Ensure this is only done on first run or when the game is reset
        Inventory.AddCash(100);
        // TODO: Let the garage scene control this!
        // create dummy first contract
        // ...requiring 5 gigatons of each elements (we might want to use floats)
        // ...and offering a payout of 300 ISA and gives 2 minutes to complete the task.
        // currentContract = new Contract( .4f, .8f, .6f, .4f, 300, 10);
        // currentContract = new Contract( .1f, .1f, .1f, .1f, 300, 120);

        // TODO: Let the garage scene control this!
        // for now, let's add a shitload of elements for free!
        /*
        if( inventory.AddElements( 5, 5, 5, 5 ) ) {
			
            Debug.Log( "WE'VE JUST GIVEN THE PLAYER ELEMENTS FOR FREE" );
        }
        */

    }

    private static GameStatus FromFileOrNew()
    {
        if (File.Exists(System.IO.Path.Combine(Application.persistentDataPath,"game.save")))
        {
            try
            {
                return ReadStatus();
            }
            catch (Exception e)
            {
                GameStatus result = new GameStatus();
                Debug.LogException(e);
                SaveStatus(result);
                return result;

            }
        }
        else
        {
            GameStatus result = new GameStatus();
            SaveStatus(result);
            return result;
        }
    }
    public static void SaveStatus() { SaveStatus(GameStatus.instance); }

    private static void SaveStatus(GameStatus status)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(GameStatus));
        StreamWriter file = new StreamWriter(System.IO.Path.Combine(Application.persistentDataPath, "game.save"));
        XmlWriter writer = XmlWriter.Create(file);
        serializer.Serialize(writer, status);
        writer.Close();
        file.Close();
    }

    private static GameStatus ReadStatus()
    {
        StreamReader file = null;
        XmlReader reader = null;
        try
        {
            XmlSerializer serializer = new XmlSerializer(typeof(GameStatus));
            file = new StreamReader(System.IO.Path.Combine(Application.persistentDataPath, "game.save"));
            reader = XmlReader.Create(file);
            GameStatus result = (GameStatus)serializer.Deserialize(reader);
            reader.Close();
            file.Close();
            if(!object.Equals(result.CurrentContract,null))
            {
                Contract c = result.CurrentContract.FindCurrentContractInGarage(result);
                if (!object.Equals(c, null))
                {
                    result.CurrentContract.finishedPlanetTexture = c.finishedPlanetTexture;
                    result.CurrentContract.customerImage = c.customerImage;
                    result.CurrentContract.level = c.level;
                }
            }
            return result;
        }
        catch (Exception e)
        {
            if (reader != null)
                reader.Close();
            if (file != null)
                file.Close();
            throw e;
        }
    }

    public bool PayElements(int earth, int fire, int water, int air)
    {
        return Inventory.PayElements(earth, fire, water, air);
    }

    public bool PayCash(int amount)
    {
        return Inventory.PayCash(amount);
    }


    /* ------------- events --------------- */

    public static void OnContractUpdate()
    {

        if (null != ContractUpdate)
        { // someone is tuned into this event

            ContractUpdate();  // broadcast the event
            GameStatus.SaveStatus();
        }
    }

    public void OnGarageLoad()
    {
        
        if (!object.Equals(CurrentContract, null) && CurrentContract.timeSpent > 0)
        {
            bool completed = true;
            foreach(Elements key in CurrentContract.requirements.Keys)
            {
                completed = completed && (CurrentContract.results[key] >= CurrentContract.requirements[key]);
            }
            if (completed)
            {
                Inventory.AddCash(CurrentContract.isaReward);
                CurrentContract.state = ContractState.COMPLETED;
                if(!CompletedContracts.Contains(CurrentContract))
                    CompletedContracts.Add(CurrentContract);

            }
            else
            {
                CurrentContract.state = ContractState.FAILED;
            }
            
        }
        if (!object.Equals(CurrentContract, null))
            OnContractUpdate(); 
        foreach (MetaContract meta in CompletedContracts)
        {
            Contract c = meta.FindContractInGarage();
            if (c != null)
            {
                GameObject.DestroyImmediate(c.gameObject);
            }
        }
        GameObject contractsRoot = GameObject.Find("contracts");
        Transform[] levels = contractsRoot.GetComponentsInChildren<Transform>(true);
        int level=0;
        GameObject levelObject = GameObject.Find("level" + level);
        int countActive = levelObject.GetComponentsInChildren<Contract>(false).Length;
        while (countActive == 0)
        {
            level++;
            foreach (Transform t in levels)
                if (t.name.Equals("level" + level))
                {
                    t.gameObject.SetActive(true);
                }
            countActive = levelObject.GetComponentsInChildren<Contract>(false).Length;
            
            
        }
        //GET ACTIVE CONTRACTS IN CHILDREN.
        //IF NONE START AT LEVEL0 AND GET CONTRACT CHILDREN (NOT GRANDCHILDREN)
        //IF NONE INCREMENT LEVEL. RINSE REPEAT UNTIL WE DO NOT FIND A LEVELX
        //if(CurrentContract != null && CurrentContract.timeSpent>0 && CurrentContract.state == ContractState.COMPLETED)
        //   currentContract = null;
        GameStatus.SaveStatus();
        GameObject.FindObjectOfType<HUDSwitch>().MaximizeHud();
    }



	////////// destination visit registration //////////

	public bool hasVisitedDestination( GarageDestinations destination ) {

		return destinationsVisited.Contains(destination);
	}

	public void markDestinationAsVisited( GarageDestinations destination ) {

		destinationsVisited.Add( destination );
		SaveStatus();
	}



    public static void Reset()
    {
        GameStatus.instance = new GameStatus();
        SaveStatus();
        OnContractUpdate();
        Inventory.OnInventoryUpdate();
        Application.LoadLevel(0);
    }
    /*
        public void OnISAUpdate() {
            if( null != ISAUpdate ) {

                ISAUpdate();
            }
        }
    */
}
