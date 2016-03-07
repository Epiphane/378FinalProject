using UnityEngine;

/**
 * Augmentation class. Has a few pieces of information:
 * - name
 * - description
 * - color
 * 
 * and hooks to be called by GameManager while resolving the turn.
 * Actions are resolved as follows:
 *   1. Call BeforeAugmentation on all augmentations (for augmentation disabling/changing/etc)
 *   2. Call BeforeAction on all augmentations (for modifying the action)
 *   3. Resolve the action
 *   4. Call AfterAction on all augmentations (to mitigate damage, reflect it, etc)
 *   5. Deal damage to players
 */
public class Card {
	public enum Color { BLANK, RED, BLUE, GREEN };

	// Visual information
	public string name { get; private set; }
	public string description { get; private set; }
	public Color color { get; private set; }

	// Converts enum color to a human readable string
	public static string ColorToString (Color color) {
		switch (color) {
		case Color.BLUE:
			return "blue";
		case Color.RED:
			return "red";
		case Color.GREEN:
			return "green";
		}

		return "ded";
	}
		
	// Information about how a turn is resolved
	public class ActionResult {
		public int damage = 0;
		public int damageToSelf = 0;
		public int advancement = 0;
	}

	// Hooks for the GameManager!
	public delegate void BeforeAugmentationHook(Card augmentation, Card other);
	public BeforeAugmentationHook BeforeAugmentation;

	public delegate void BeforeActionHook(PlayerAction action);
	public BeforeActionHook BeforeAction;

	public delegate void AfterActionHook(ActionResult result, PlayerScript player, PlayerScript other);
	public AfterActionHook AfterAction;

	// Constructor takes all of this information
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

	/*
	 * This is the running DB of all the cards. To add a new one, just append it to this array!
	 * Be careful to put the right callback in the right spot!
	 */
	public static Card[] cards = {
		new Card ("Justice", "All damage you take this turn is dealt to your opponent too", Color.GREEN, null, null, (ActionResult result, PlayerScript player, PlayerScript other) => {
			other.health -= result.damageToSelf;
		}),
		new Card ("Kindness", "Heal 2 after this turn", Color.GREEN, null, null, (ActionResult result, PlayerScript player, PlayerScript other) => {
			player.health += 2;
		}),
		new Card ("Bloodlust", "Deal double damage", Color.RED, null, (PlayerAction action) => {
			action.physicalAttack *= 2;
			action.techAttack *= 2;
		}, null),
		new Card ("Feast", "Heal half the damage you deal", Color.RED, null, null, (ActionResult result, PlayerScript player, PlayerScript other) => {
			player.health += (int) Mathf.Floor(result.damage / 2);
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