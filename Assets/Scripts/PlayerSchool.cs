using UnityEngine;
using System.Collections;

public class PlayerSchool {

	public string name { get; private set; }
	public Color color { get; private set; }
	public Level[] levels { get; private set; }
	public int advancement;

	// Hooks for the GameManager!
	public delegate void BeforeAugmentationHook(Card augmentation, Card other);
	public delegate void BeforeActionHook(PlayerAction action);
	public delegate void AfterActionHook(Card.ActionResult result, Card.ActionResult other);

	public class Level {
		public string description;

		public BeforeAugmentationHook BeforeAugmentation;
		public BeforeActionHook BeforeAction;
		public AfterActionHook AfterAction;

		public Level(string description, BeforeAugmentationHook beforeAug, BeforeActionHook beforeAct, AfterActionHook afterAct) {
			this.description = description;
			BeforeAugmentation = beforeAug;
			BeforeAction = beforeAct;
			AfterAction = afterAct;
		}
	}

	public PlayerSchool(string name, Color color, Level[] levels) {
		this.name = name;
		this.color = color;
		this.levels = levels;
		advancement = 0;
	}

	public void BeforeAugmentation(Card augmentation, Card other) {
		for (int i = 0; i < 3 && i * 6 <= advancement; i ++) {
			if (this.levels [i].BeforeAugmentation != null)
				this.levels [i].BeforeAugmentation (augmentation, other);
		}
	}

	public void BeforeAction(PlayerAction action) {
		for (int i = 0; i < 3 && i * 6 <= advancement; i ++) {
			if (this.levels [i].BeforeAction != null)
				this.levels [i].BeforeAction (action);
		}
	}

	public void AfterAction(Card.ActionResult result, Card.ActionResult other) {
		for (int i = 0; i < 3 && i * 6 <= advancement; i ++) {
			if (this.levels [i].AfterAction != null)
				this.levels [i].AfterAction (result, other);
		}
	}

	public PlayerSchool Clone() {
		return new PlayerSchool (name, color, levels);
	}

	public static PlayerSchool[] schools = {
		new PlayerSchool ("School of Aggression", Color.red, new Level[] {
			new Level ("Nothing", null, null, null),
			new Level ("+1 attack and tech", null, (PlayerAction action) => {
				if (action.name == "Attack") {
					action.physicalAttack++;
				} else if (action.name == "Tech") {
					action.techAttack++;
				}
			}, null),
			new Level ("+2 attack and tech", null, (PlayerAction action) => {
				if (action.name == "Attack") {
					action.physicalAttack++;
				} else if (action.name == "Tech") {
					action.techAttack++;
				}
			}, null),
			new Level ("Nothing", null, null, null)
		}),
		new PlayerSchool ("School of Tactics", Color.red, new Level[] {
			new Level ("Nothing", null, null, null),
			new Level ("Always choose first augmentation", null, (PlayerAction action) => {
				// TODO
			}, null),
			new Level ("+1 damage to tech and counter", null, (PlayerAction action) => {
				if (action.name == "Counter") {
					action.counterAttack ++;
				} else if (action.name == "Tech") {
					action.techAttack++;
				}
			}, null),
			new Level ("Your opponent plays their augmentation first", null, null, null)
		}),
		new PlayerSchool ("School of Focus", Color.red, new Level[] {
			new Level ("Nothing", null, null, null),
			new Level ("+1 AP on advance", null, (PlayerAction action) => {
				if (action.name == "Advance")
					action.advancement ++;
			}, null),
			new Level ("Heal 1 on counter", null, null, (Card.ActionResult result, Card.ActionResult other) => {
				if (result.action.name == "Counter")
					other.damage --;

				// TODO add an actual HEAL
			}),
			new Level ("Take 3 damage maximum in an action", null, null, (Card.ActionResult result, Card.ActionResult other) => {
				other.damage = Mathf.Min(other.damage, 3);
			})
		})
	};
}
