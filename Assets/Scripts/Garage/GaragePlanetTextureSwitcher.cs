using UnityEngine;
using System.Collections;

public class GaragePlanetTextureSwitcher : MonoBehaviour {

    public Material defaultMaterial;
	// Use this for initialization
	void Start () {
		// listen to current contract change events:
		GameStatus.ContractUpdate += SetTextureFromCurrentContract;
	}

	void OnDestroy() {

		GameStatus.ContractUpdate -= SetTextureFromCurrentContract;
	}


	private void SetTextureFromCurrentContract() {
        //renderer.materials[0] = GameStatus.instance.CurrentContract.finishedPlanetTexture;
        if(GameStatus.instance.CurrentContract != null)
		    GetComponent<Renderer>().material.SetTexture("_MainTex", GameStatus.instance.CurrentContract.finishedPlanetTexture);
	}
}
