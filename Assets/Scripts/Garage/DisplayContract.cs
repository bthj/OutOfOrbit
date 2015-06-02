using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DisplayContract : TouchBehaviour {

    public Contract contract;

	// Update is called once per frame
	void Update () {
        TextMesh[] contractTexts = GetComponentsInChildren<TextMesh>();
        foreach(TextMesh mesh in contractTexts)
        {
            switch(mesh.name)
            {
                case "Label1": 
                    mesh.text = contract.Order[0]+"";
                    break;
                case "Label2":
                    mesh.text = contract.Order[1] + "";
                    break;
                case "Label3":
                    mesh.text = contract.Order[2] + "";
                    break;
                case "Label4":
                    mesh.text = contract.Order[3] + "";
                    break;
                case "Label5":
                    mesh.text = "I$A:";
                    break;
                case "Label6":
                    mesh.text = "Time:";
                    break;
                case "Amount1":
					mesh.text = contract.requirements[contract.Order[0]] - contract.results[contract.Order[0]] + "";
                    break;
                case "Amount2":
					mesh.text = contract.requirements[contract.Order[1]] - contract.results[contract.Order[1]] + "";
                    break;
                case "Amount3":
					mesh.text = contract.requirements[contract.Order[2]] - contract.results[contract.Order[2]] + "";
                    break;
                case "Amount4":
					mesh.text = contract.requirements[contract.Order[3]] - contract.results[contract.Order[3]] + "";
                    break;
                case "Amount5":
                    mesh.text = contract.isaReward+ "";
                    break;
                case "Amount6":
                    mesh.text = contract.timeLimit + "";
                    break;
                default:
                    break;
            }
        }
	}

    public override void OnTouchEnd(TouchController tc, int touchIndex, Vector2 position)
    {
        Ray r = Camera.main.ScreenPointToRay(position);
        RaycastHit hit;
        if(Physics.Raycast(r,out hit, tc.mask))
        {
            if (hit.collider == gameObject.GetComponent<Collider>())
            {
                GameStatus.instance.CurrentContract = MetaContract.FromContract(contract);

            }
        }
    }
}
