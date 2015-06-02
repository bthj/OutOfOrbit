using UnityEngine;
using System.Collections;
using System.Text;

public class HUDController : MonoBehaviour
{

    public TextMesh generalInfo;
    private string intro;
    public GameObject generalInfoImage;

    public TextMesh bankStatus;
    public TextMesh fireInventory, earthInventory, waterInventory, airInventory;

	public string contractsGuide, portalGuide, shopGuide, windowGuide;
	public string contractMissingGuide, elementsMissingGuide;

	public int stampWait = 2;

    private HUDSwitch hudSwitch;
	private DestinationBroadcast destinationBroadcast;

    // Use this for initialization
    void Start()
    {

        hudSwitch = gameObject.GetComponent<HUDSwitch>();
        intro = generalInfo.text;
        // subscribe to events
        GameStatus.ContractUpdate += DisplayCurrentContract;
        //		GameStatus.instance.ISAUpdate += new GameStatus.GameStatusUpdateDelegate( DisplayISABalance );
        Inventory.InventoryUpdate += DisplayInventory;

        DisplayInventory();
        if (!object.Equals(GameStatus.instance, null) && !object.Equals(GameStatus.instance.CurrentContract, null))
            RenderCurrentContract();

		// listen to destination events
		destinationBroadcast = 
			GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<DestinationBroadcast>();
		destinationBroadcast.destinationUpdate += HandleDestinationUpdate;
    }


    public void RenderCurrentContract()
    {
		string currentConractString;
        MetaContract contract = GameStatus.instance.CurrentContract;

		if( null != contract ) {

			generalInfo.tabSize = 5;
			switch (contract.state)
			{
			case ContractState.ACCPETED:
				currentConractString =  AcceptedContract(contract);
				break;
			case ContractState.COMPLETED:
				currentConractString = CompletedContract(contract);
				break;
			case ContractState.FAILED:
				currentConractString = FailedContract(contract);
				break;
			default:
				currentConractString = intro;
				break;
			}
			generalInfo.text = currentConractString;
		}
	}
	
	private string AcceptedContract(MetaContract contract)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("Contract: " + contract.contractName).AppendLine();
        sb.Append("Client: " + contract.client).AppendLine();
        sb.Append("Obstacles: ");
        if (contract.Obstacles.Count == 0)
            sb.Append("No obstacles").AppendLine();
        else
        {
            foreach (Obstacles o in contract.Obstacles)
                sb.Append(o + ", ");
            sb.Remove(sb.Length - 2, 2);
            sb.AppendLine();
        }
        //sb.Append( contract.obstacles ).AppendLine();
        sb.Append("Notes: " + contract.flavourText).AppendLine();
        sb.AppendLine();
        sb.AppendLine();
        sb.Append("Fire:  ").Append(contract.requirements[Elements.FIRE] - contract.startingElements[Elements.FIRE]).Append(" Gt.").AppendLine();
        sb.Append("Earth:  ").Append(contract.requirements[Elements.EARTH] - contract.startingElements[Elements.EARTH]).Append(" Gt.").AppendLine();
        sb.Append("Water:  ").Append(contract.requirements[Elements.WATER] - contract.startingElements[Elements.WATER]).Append(" Gt.").AppendLine();
        sb.Append("Air:  ").Append(contract.requirements[Elements.AIR] - contract.startingElements[Elements.AIR]).Append(" Gt.").AppendLine();
        sb.AppendLine();
        sb.Append("I$A reward:  ").Append(contract.isaReward).AppendLine();
        sb.Append("Work time:  ").Append(contract.timeLimit).AppendLine();

        return sb.ToString();
    }

    private string CompletedContract(MetaContract contract)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("Contract Overview:").AppendLine();
        sb.Append("Name: "+contract.contractName).AppendLine();
        sb.Append("Time Spent: " + (int)contract.timeSpent + "s / " + contract.timeLimit + "s").AppendLine();
        sb.Append("I$A Earned: "+contract.isaReward).AppendLine();
        sb.Append("Message From Client: " + contract.successMessage);
        return sb.ToString();
    }

    private string FailedContract(MetaContract contract)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("Contract Overview:").AppendLine();
        sb.Append("Name: " + contract.contractName).AppendLine();
        sb.Append("Time Spent: " + (int)contract.timeSpent + "s / " + contract.timeLimit + "s").AppendLine();
        sb.Append("Message From Client: " + contract.failureMessage);
        return sb.ToString();
    }
    // ---- event handlers -----

    private void DisplayCurrentContract()
    {

        // Bring up the HUD 
        hudSwitch.MaximizeHud();

        // and write the current contract info to it
        if (GameStatus.instance.CurrentContract != null)
            RenderCurrentContract(); // TODO: animated typing with coroutine
        else
            generalInfo.text = intro;

		StartCoroutine( ContractStamp() );
    }
	
	private void DisplayInventory()
	{
		Inventory inventory = GameStatus.instance.Inventory;
		
		fireInventory.text = "Fire: " + inventory.Fire;
		earthInventory.text = "Earth: " + inventory.Earth;
		waterInventory.text = "Water: " + inventory.Water;
		airInventory.text = "Air: " + inventory.Air;

        bankStatus.text = "Money in the bank: " + inventory.Cash + ",- I$A";
    }



	///// stamping /////
	
	private IEnumerator ContractStamp() {

		HideContractStamps();
		
		if (GameStatus.instance.CurrentContract != null)
		{
			yield return new WaitForSeconds( stampWait );
			
			bool wasStampRendered = RenderContractStamps();
			if( wasStampRendered ) GetComponent<AudioSource>().Play();
		}
	}

	public bool RenderContractStamps() {

		bool stampRendered = false;

		if( null != GameStatus.instance.CurrentContract ) {

			switch (GameStatus.instance.CurrentContract.state)
			{
			case ContractState.ACCPETED:
				GameObject.Find("Stamps/AcceptedStamp").GetComponent<Renderer>().enabled = true;
				GameObject.Find("Stamps/CompletedStamp").GetComponent<Renderer>().enabled = false;
				GameObject.Find("Stamps/FailedStamp").GetComponent<Renderer>().enabled = false;
				stampRendered = true;
				break;
			case ContractState.COMPLETED:
				GameObject.Find("Stamps/AcceptedStamp").GetComponent<Renderer>().enabled = false;
				GameObject.Find("Stamps/CompletedStamp").GetComponent<Renderer>().enabled = true;
				GameObject.Find("Stamps/FailedStamp").GetComponent<Renderer>().enabled = false;
				stampRendered = true;
				break;
			case ContractState.FAILED:
				GameObject.Find("Stamps/AcceptedStamp").GetComponent<Renderer>().enabled = false;
				GameObject.Find("Stamps/CompletedStamp").GetComponent<Renderer>().enabled = false;
				GameObject.Find("Stamps/FailedStamp").GetComponent<Renderer>().enabled = true;
				stampRendered = true;
				break;
			default:
				GameObject.Find("Stamps/AcceptedStamp").GetComponent<Renderer>().enabled = false;
				GameObject.Find("Stamps/CompletedStamp").GetComponent<Renderer>().enabled = false;
				GameObject.Find("Stamps/FailedStamp").GetComponent<Renderer>().enabled = false;
				break;
			}
		}

		return stampRendered;
	}

	private void HideContractStamps() {

		GameObject.Find("Stamps/AcceptedStamp").GetComponent<Renderer>().enabled = false;
		GameObject.Find("Stamps/CompletedStamp").GetComponent<Renderer>().enabled = false;
		GameObject.Find("Stamps/FailedStamp").GetComponent<Renderer>().enabled = false;
	}



	private void HandleDestinationUpdate( GarageDestinations broadcastDestination ) {

		if( ! GameStatus.instance.hasVisitedDestination(broadcastDestination) ) {

			string guideText = GetGuideForDestination( broadcastDestination );
			if( guideText != string.Empty ) {

				generalInfo.text = guideText;
				HideContractStamps();
				hudSwitch.MaximizeHud();
			}
			GameStatus.instance.markDestinationAsVisited( broadcastDestination );
		}
	}

	private string GetGuideForDestination( GarageDestinations destination ) {
		string guideText;
		switch( destination ) {
		case GarageDestinations.CONTRACTS:
			guideText = contractsGuide;
			break;
		case GarageDestinations.PORTAL:
			guideText = portalGuide;
			break;
		case GarageDestinations.SHOP:
			guideText = shopGuide;
			break;
		case GarageDestinations.WINDOW:
			guideText = windowGuide;
			break;
		default:
			guideText = "";
			break;
		}
		return guideText;
	}

	public void ShowContractMissingGuide() {
		generalInfo.text = contractMissingGuide;
		HideContractStamps();
		hudSwitch.MaximizeHud();
	}
	public void ShowElementsMissingGuide() {
		generalInfo.text = elementsMissingGuide;
		HideContractStamps();
		hudSwitch.MaximizeHud();
	}


    void OnDestroy()
    {
        Inventory.InventoryUpdate -= DisplayInventory;
        GameStatus.ContractUpdate -= DisplayCurrentContract;
		destinationBroadcast.destinationUpdate -= HandleDestinationUpdate;
    }
}
