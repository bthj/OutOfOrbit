﻿using UnityEngine;
using System.Collections;

public class YRotate : MonoBehaviour {

	public int rotationSpeed = 1;

	// Update is called once per frame
	void Update () {
	
		transform.Rotate(Vector3.up * Time.deltaTime * rotationSpeed, Space.Self);
	}
}