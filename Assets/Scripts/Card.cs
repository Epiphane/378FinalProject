﻿using UnityEngine;

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
	public bool chainable { get; private set; }
	public Card previous;

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
		public int healing = 0;
		public int advancement = 0;
		public int extraCards = 0;
		public PlayerAction action;
	}

	// Hooks for the GameManager!
	public delegate void InstantHook(Card augmentation, Card other);
	public InstantHook Instant;

	public delegate void BeforeActionHook(PlayerAction action);
	public BeforeActionHook BeforeAction;

	public delegate void AfterActionHook(ActionResult result, ActionResult other);
	public AfterActionHook AfterAction;
	public AfterActionHook AfterActionBeforeSchool;

	// Constructor takes all of this information
	public Card(string name, string description, Color color, bool chainable, InstantHook Instant,
		BeforeActionHook BeforeAction, AfterActionHook AfterActionBeforeSchool, AfterActionHook AfterAction) {
		this.name = name;
		this.color = color;
		this.description = description;
		this.chainable = chainable;
		this.previous = null;

		// Fill in nulls
		if (Instant == null)
			Instant = (Card augmentation, Card other) => {};
		if (BeforeAction == null)
			BeforeAction = (PlayerAction action) => {};
		if (AfterActionBeforeSchool == null)
			AfterActionBeforeSchool = (ActionResult result, ActionResult other) => {};
		if (AfterAction == null)
			AfterActionBeforeSchool = (ActionResult result, ActionResult other) => {};

		this.Instant = Instant;
		this.BeforeAction = BeforeAction;
		this.AfterActionBeforeSchool = AfterActionBeforeSchool;
		this.AfterAction = AfterAction;
	}

	public Card Clone () {
		return new Card (name, description, color, chainable, Instant, BeforeAction, AfterActionBeforeSchool, AfterAction);
	}

	public override string ToString () {
		return name;
	}

	/*
	 * This is the running DB of all the cards. To add a new one, just append it to this array!
	 * Be careful to put the right callback in the right spot!
	 */
	public static Card[] cards = {
		new Card ("Justice", "All damage you take this turn is dealt to your opponent too", Color.GREEN, false, null, null, null, (ActionResult result, ActionResult other) => {
			result.damage += other.damage;
		}),
		new Card ("Kindness", "Heal 1 after this action", Color.GREEN, true, null, null, null, (ActionResult result, ActionResult other) => {
			other.healing ++;
		}),
		new Card ("Determination", "Counter will block ALL damage", Color.GREEN, false, null, (PlayerAction action) => {
			if (action.name == "Counter") {
				action.defense = 100;
			}
		}, null, null),
		new Card ("Patience", "+2 damage to counter, +2 AP to advance", Color.GREEN, false, null, (PlayerAction action) => {
			if (action.name == "Counter") {
				action.counterAttack += 2;
			}
			else if (action.name == "Advance") {
				action.advancement += 2;
			}
		}, null, null),
		new Card ("Bloodlust", "Deal double damage", Color.RED, false, null, (PlayerAction action) => {
			action.physicalAttack *= 2;
			action.techAttack *= 2;
		}, null, null),
		new Card ("Feast", "Draw one extra card for each damage you deal (max 3)", Color.RED, false, null, null, null, (ActionResult result, ActionResult other) => {
			result.extraCards = Mathf.Min(result.damage, 3);
		}),
		new Card ("Learn by Doing", "Gain 1AP for each damage you deal", Color.RED, false, null, null, null, (ActionResult result, ActionResult other) => {
			result.advancement += result.damage;
		}),
		new Card ("Quick Attack", "Deal 1 extra damage after this action", Color.RED, true, null,  null, null, (ActionResult result, ActionResult other) => {
			result.damage ++;
		}),
		new Card ("Morph", "Copy your opponent's augmentation", Color.BLUE, false, (Card augmentation, Card other) => {
			augmentation.chainable = other.chainable;
			augmentation.BeforeAction = other.BeforeAction;
			augmentation.AfterAction = other.AfterAction;
			augmentation.AfterActionBeforeSchool = other.AfterActionBeforeSchool;
		}, null, null, null),
		new Card ("Mad Hacks", "Cancel your opponent's augmentation", Color.BLUE, false, (Card augmentation, Card other) => {
			other.chainable = false;
			other.BeforeAction = null;
			other.AfterAction = null;
			other.AfterActionBeforeSchool = null;
		}, null, null, null),
		new Card ("Siphon", "If tech deals damage, steal 2AP from your opponent", Color.BLUE, false, null, null, null, (ActionResult result, ActionResult other) => {
			if (result.damage > 0 && result.action.name == "Tech") {
				other.advancement -= 2;
				result.advancement += 2;
			}
		}),
		new Card ("Mind games", "+1 damage to tech", Color.BLUE, false, null, (PlayerAction action) => {
			if (action.name == "Tech")
				action.techAttack ++;
		}, null, null)
	};
}