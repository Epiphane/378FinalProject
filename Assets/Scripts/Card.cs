using UnityEngine;

public class Card {
	public enum Color { BLANK, RED, BLUE, GREEN };

	public string name { get; private set; }
	public string description { get; private set; }
	public Color color { get; private set; }

	public static string ColorToString (Color color) {
		switch (color) {
		case Color.BLUE:
			return "blue";
			break;
		case Color.RED:
			return "red";
			break;
		case Color.GREEN:
			return "green";
			break;
		}

		return "ded";
	}

	public class ActionResult {
		public int damage = 0;
		public int damageToSelf = 0;
		public int advancement = 0;
	}

	public delegate void BeforeAugmentationHook(Card augmentation, Card other);
	public BeforeAugmentationHook BeforeAugmentation;

	public delegate void BeforeActionHook(PlayerAction action);
	public BeforeActionHook BeforeAction;

	public delegate void AfterActionHook(ActionResult result, PlayerScript player, PlayerScript other);
	public AfterActionHook AfterAction;

	public Card(string name, string description, Color color, BeforeAugmentationHook BeforeAugmentation,
		BeforeActionHook BeforeAction, AfterActionHook AfterAction) {
		this.name = name;
		this.color = color;
		this.description = description;

		this.BeforeAugmentation = BeforeAugmentation;
		this.BeforeAction = BeforeAction;
		this.AfterAction = AfterAction;
	}

	public Card Clone () {
		return new Card (name, description, color, BeforeAugmentation, BeforeAction, AfterAction);
	}

	public override string ToString () {
		return name;
	}

	public static Card[] cards = {
		new Card ("Justice", "All damage you take this turn is dealt to your opponent too", Color.GREEN, null, null, (ActionResult result, PlayerScript player, PlayerScript other) => {
			other.health -= result.damageToSelf;
			Debug.Log("Dealing " + result.damageToSelf + " back");
		}),
		new Card ("Kindness", "Heal 2 after this turn", Color.GREEN, null, null, (ActionResult result, PlayerScript player, PlayerScript other) => {
			player.health += 2;
			Debug.Log("Healing 2");
		}),
		new Card ("Bloodlust", "Deal double damage", Color.RED, null, (PlayerAction action) => {
			action.physicalAttack *= 2;
			action.techAttack *= 2;
		}, null),
		new Card ("Feast", "Heal half the damage you deal", Color.RED, null, null, (ActionResult result, PlayerScript player, PlayerScript other) => {
			player.health += (int) Mathf.Floor(result.damage / 2);
			Debug.Log("Healing " + (int) Mathf.Floor(result.damage / 2));
		}),
		new Card ("Morph", "Copy your opponent's augmentation", Color.BLUE, (Card augmentation, Card other) => {
			augmentation.BeforeAction = other.BeforeAction;
			augmentation.AfterAction = other.AfterAction;
		}, null, null),
		new Card ("Mad Hacks", "Cancel your opponent's augmentation", Color.BLUE, (Card augmentation, Card other) => {
			other.BeforeAction = null;
			other.AfterAction = null;
		}, null, null),
	};
}