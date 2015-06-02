using UnityEngine;
using System.Collections;

public class DestinationBroadcast : MonoBehaviour
{

    public delegate void DestinationUpdateHandler(GarageDestinations destination);
    public event DestinationUpdateHandler destinationUpdate;
    private bool done = false;

    void Update()
    {
        if (!done && !object.Equals(GameStatus.instance,null))
        {
            done = true;
            GameStatus.instance.OnGarageLoad();
            if(!object.Equals(GameStatus.instance.CurrentContract,null))
            {
                Contract c = GameStatus.instance.CurrentContract.FindCurrentContractInGarage(GameStatus.instance);
                if(c != null)
                    Debug.Log("Matches: " + c.contractName);
            }
        }
    }

    public void BroadcastDestination(GarageDestinations destinationName)
    {

        OnDestinationUpdate(destinationName);
    }


    private void OnDestinationUpdate(GarageDestinations destinationName)
    {

        if (null != destinationUpdate) destinationUpdate(destinationName);
    }
}
