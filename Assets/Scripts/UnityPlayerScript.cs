using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UnityPlayerScript : PlayerScript {

	/* For unity UI */
	public GameObject actions;

	// Use this for initialization
	public override void Awake () {
		base.Awake ();

		hand.OnPickCard = PickCard;
	}

	public void PickCard(Card card) {
		if (gameManager.CanPickAugmentation (ID)) {
			PlayAugmentation (card);
		}
	}
}
