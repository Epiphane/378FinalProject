using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {

	public CardHolderScript hand;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void DrawCard(Card card) {
		hand.AddCard (card);
	}
}
