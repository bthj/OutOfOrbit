using UnityEngine;
using System.Collections;

public class StartMiniGame : TouchBehaviour {

	private HUDController hudController;

	void Start() {

		hudController = GameObject.Find("HUD").GetComponent<HUDController>();
	}

	public override void OnTouchEnd(TouchController tc, int touchIndex, Vector2 position)
	{
        Ray r = Camera.main.ScreenPointToRay(position);
        RaycastHit hit;
        if (Physics.Raycast(r, out hit, tc.mask))
        {
            if (hit.collider == gameObject.GetComponent<Collider>())
                loadMiniGame();
        }
    }


    void loadMiniGame()
    {

        if (GameStatus.instance.CurrentContract != null && GameStatus.instance.CurrentContract.state!=ContractState.COMPLETED && checkRequirements())
        {
            GameStatus.SaveStatus();
            GameObject portalCam = GameObject.Find("Cameras/Portal");
            portalCam.GetComponent<Animator>().SetBool("start", true);
            Camera portal = portalCam.GetComponent<Camera>();
            Camera.main.enabled = false;
            portal.enabled = true;
        } else {
			if( GameStatus.instance.CurrentContract == null ) {
				hudController.ShowContractMissingGuide();
			} else if (GameStatus.instance.CurrentContract.state == ContractState.COMPLETED)
            {
                hudController.ShowContractMissingGuide();
            }
            else if( ! checkRequirements() ) {
				hudController.ShowElementsMissingGuide();
			}
		}
    }

    private bool checkRequirements()
    {
        bool okay = true;
        foreach(Elements key in GameStatus.instance.CurrentContract.requirements.Keys)
        {
            okay = okay && (GameStatus.instance.CurrentContract.requirements[key] - GameStatus.instance.CurrentContract.startingElements[key] == 0 || GameStatus.instance.Inventory.GetElementAmount(key) >= GameStatus.instance.CurrentContract.requirements[key] - GameStatus.instance.CurrentContract.startingElements[key] + 1);
        }
        return okay;
    }
}
