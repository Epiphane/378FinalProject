using UnityEngine;

public class Card {
	public enum Color { BLANK, RED, BLUE, GREEN };

	public string name { get; private set; }
	public string description { get; private set; }
	public Color color { get; private set; }

	public Card(string name, string description, Color color) {
		this.name = name;
		this.color = color;
		this.description = description;
	}

	public Card Clone () {
		return new Card (name, description, color);
	}

	public override string ToString () {
		return name;
	}

	public static Card[] cards = {
		new Card ("Justice", "All damage you take this turn is dealt to your opponent too", Color.GREEN),
		new Card ("Kindness", "Heal 2 after this turn", Color.GREEN),
		new Card ("Bloodlust", "Deal double damage", Color.RED),
		new Card ("Feast", "Heal half the damage you deal", Color.RED),
		new Card ("Morph", "Copy your opponent's augmentation", Color.BLUE),
		new Card ("Mad Hacks", "Cancel your opponent's augmentation", Color.BLUE),
	};
}