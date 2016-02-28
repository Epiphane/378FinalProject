using UnityEngine;
using System.Collections;

public class Deck {
	public static Card RandomCard() {
		// Decide on a type
		int cType = Random.Range (0, 2);
		Card.Type t = Card.Type.ATTACK;
		if (cType == 1)
			t = Card.Type.SPELL;
		else if (cType == 2)
			t = Card.Type.BLOCK;

		// Decide on a color
		int cColor = Random.Range (0, 2);
		Card.Color c = Card.Color.RED;
		if (cColor == 1)
			c = Card.Color.BLUE;
		else if (cColor == 2)
			c = Card.Color.GREEN;

		return new Card (t, c);
	}

}