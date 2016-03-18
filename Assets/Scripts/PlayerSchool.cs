using UnityEngine;
using System.Collections;

public class PlayerSchool {

	public string name { get; private set; }
	public Color color { get; private set; }
	public Level[] levels { get; private set; }
	public int advancement;
	public Card[] starters;

	// Hooks for the GameManager!
	public delegate void BeforeAugmentationHook(Card augmentation, Card other);
	public delegate void BeforeActionHook(PlayerAction action);
	public delegate void AfterActionHook(Card.ActionResult result, Card.ActionResult other);

	public class Level {
		public string description;

		public BeforeAugmentationHook BeforeAugmentation;
		public BeforeActionHook BeforeAction;
		public AfterActionHook AfterAction;
        public bool secondMove;
        public bool firstPick;

		public Level(string description, BeforeAugmentationHook beforeAug, BeforeActionHook beforeAct, AfterActionHook afterAct, bool firstPick = false, bool secondMove = false) {
			this.description = description;
			BeforeAugmentation = beforeAug;
			BeforeAction = beforeAct;
			AfterAction = afterAct;
            this.firstPick = firstPick;
            this.secondMove = secondMove;

        }
	}

	public PlayerSchool(string name, Color color, Level[] levels, Card[] starters) {
		this.name = name;
		this.color = color;
		this.levels = levels;
		this.starters = starters;
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

    public bool FirstPick()
    {
        for (int i = 0; i < 3 && i * 6 <= advancement; i++)
            if (this.levels[i].firstPick)
                return true;

		return false;
	}

	public void GenerateDeck(DeckScript deckScript) {
		foreach (Card card in starters) {
			deckScript.AddCard (card.Clone ());
		}
	}

    public bool SecondMove()
    {
        for (int i = 0; i < 4 && i * 6 <= advancement; i++)
            if (this.levels[i].secondMove)
                return true;

        return false;
    }


	public PlayerSchool Clone() {
		return new PlayerSchool (name, color, levels, starters);
	}

	public static PlayerSchool[] schools = {
		new PlayerSchool ("Aggression: Destroy your enemy", Color.red, new Level[] {
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
		}, new Card[] {
				Card.cards[4],
				Card.cards[4],
				Card.cards[5],
				Card.cards[6],
				Card.cards[6],
				Card.cards[7],
		}),
		new PlayerSchool ("Tactics: Outwit and outplay", Color.blue, new Level[] {
			new Level ("Nothing", null, null, null),
			new Level ("Always choose first augmentation", null, null, null, true),
			new Level ("+1 tech and counter", null, (PlayerAction action) => {
				if (action.name == "Counter") {
					action.counterAttack ++;
				} else if (action.name == "Tech") {
					action.techAttack++;
				}
			}, null),
			new Level ("Your opponent plays their augmentation first", null, null, null, false, true)
		}, new Card[] {
				Card.cards[8],
				Card.cards[8],
				Card.cards[9],
				Card.cards[10],
				Card.cards[10],
				Card.cards[11],
		}),
		new PlayerSchool ("Focus: Steadfast in victory", Color.green, new Level[] {
			new Level ("Nothing", null, null, null),
			new Level ("+1 AP on advance", null, (PlayerAction action) => {
				if (action.name == "Advance")
					action.advancement ++;
			}, null),
			new Level ("Heal 1 on counter", null, null, (Card.ActionResult result, Card.ActionResult other) => {
				if (result.action.name == "Counter")
					other.healing ++;
			}),
			new Level ("Take 3 damage maximum in an action", null, null, (Card.ActionResult result, Card.ActionResult other) => {
				other.damage = Mathf.Min(other.damage, 3);
			})
		}, new Card[] {
				Card.cards[0],
				Card.cards[1],
				Card.cards[1],
				Card.cards[2],
				Card.cards[2],
				Card.cards[3],
		})
	};
}
