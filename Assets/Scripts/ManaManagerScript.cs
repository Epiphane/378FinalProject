using UnityEngine;
using System.Collections;

public class ManaManagerScript : MonoBehaviour {

	public ManaBarScript redBar;
	public ManaBarScript blueBar;
	public ManaBarScript greenBar;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Clear() {
		redBar.val = blueBar.val = greenBar.val = 0;
	}

	/* Increment mana for a specific color and return whether it's a combo */
	public bool Increment(Card.Color color) {
		switch (color) {
		case Card.Color.RED:
			return redBar.Increment ();
		case Card.Color.BLUE:
			return blueBar.Increment ();
		case Card.Color.GREEN:
			return greenBar.Increment ();
		}

		return false;
	}
}
