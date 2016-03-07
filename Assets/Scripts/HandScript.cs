using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HandScript : CardHolderScript {

	private static float FAN_WIDTH = 1;

	// Use this for initialization
	void Start () {
	}

	public override void CardSelected (Transform transform, Card card) {
		OnPickCard (card);
	}

	public override Vector3 StartingPoint() {
		return transform.position - Vector3.up * 100;
	}

	public override void Reorganize () {
		for (int i = 0; i < cardTransforms.Count; i++) {
			float tilt = (cardTransforms.Count - 2.0f * i - 1) * FAN_WIDTH;
			float xval = tilt * -200 / cardTransforms.Count;

			UISmoothTransformScript cTransform = cardTransforms [i].GetComponent<UISmoothTransformScript> ();

			cTransform.Record ();

			// Set anchor
			cardTransforms [i].transform.position = transform.position + new Vector3(xval, 0, -i / 100.0f);
			cardTransforms [i].transform.rotation = Quaternion.Euler(new Vector3 (0, 0, 0));
			cardTransforms [i].transform.RotateAround (transform.position + new Vector3(xval, -10, 0), Vector3.forward, tilt);
		
			cTransform.StartTransition(0.5f);
		}
	}
	
	// Update is called once per frame
	void Update () {
	}
}
