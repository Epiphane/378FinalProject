using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {

	public CardHolderScript hand;
	public int ID;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public virtual void DrawCard(Card card) {
		hand.AddCard (card);
	}
}
