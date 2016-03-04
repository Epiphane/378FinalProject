using UnityEngine;
using System.Collections;

public class PlayerAction {

	public static PlayerAction[] actions = {
		new PlayerAction("Attack", 4, 0, 0, 0),
		new PlayerAction("Tech", 0, 2, 0, 0),
		new PlayerAction("Counter", 0, 0, 2, 0),
		new PlayerAction("Advance", 0, 0, 0, 3)
	};

	public string name;
	public int physicalAttack = 0;
	public int techAttack = 0;
	public int counterAttack = 0;
	public int advancement = 0;

	public PlayerAction(string name, int pAtk, int tAtk, int cAtk, int adv) {
		this.name = name;
		physicalAttack = pAtk;
		techAttack = tAtk;
		counterAttack = cAtk;
		advancement = adv;
	}

	public PlayerAction Clone() {
		return new PlayerAction (name, physicalAttack, techAttack, counterAttack, advancement);
	}

	public static PlayerAction GetRandom() {
		return actions [Random.Range (0, actions.Length - 1)].Clone();
	}
}
