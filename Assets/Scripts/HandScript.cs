using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HandScript : CardHolderScript {

	private static float FAN_WIDTH = 1;

	public bool fan = true;

	// Use this for initialization
	void Start () {
	}

	public override void CardSelected (Transform transform, Card card) {
		RemoveCard (card);
	}

	public override void Reorganize () {
		for (int i = 0; i < cardTransforms.Count; i++) {
			float tilt = (cardTransforms.Count - 2.0f * i - 1) * FAN_WIDTH;
			float xval = tilt * -3 / cardTransforms.Count;
			// Set anchor
			cardTransforms [i].transform.position = transform.position + new Vector3(xval, 0, -i / 100.0f);
			cardTransforms [i].transform.rotation = Quaternion.Euler(new Vector3 (0, 0, 0));
			cardTransforms [i].transform.RotateAround (transform.position + new Vector3(xval, -10, 0), Vector3.forward, tilt);
		}
	}
	
	// Update is called once per frame
	void Update () {
	}
}
