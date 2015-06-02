using UnityEngine;
using System.Collections;

public class HashIDs : MonoBehaviour {

	public int completedContract;
	public int timeIsUp;
	public int intoOrbit;
	public int isHudUp;

	void Awake() {

		completedContract = Animator.StringToHash("completedContract");
		timeIsUp = Animator.StringToHash("timeIsUp");
		intoOrbit = Animator.StringToHash("intoOrbit");
		isHudUp = Animator.StringToHash("isHudUp");
	}
}
