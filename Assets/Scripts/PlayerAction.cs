using UnityEngine;
using System.Collections;

public class PlayerAction {

	public static PlayerAction[] actions = {
		new PlayerAction("Attack", 4, 0, 0, 0, 0),
		new PlayerAction("Tech", 0, 0, 2, 0, 0),
		new PlayerAction("Counter", 0, 4, 0, 0, 0),
		new PlayerAction("Advance", 0, 0, 0, 0, 3)
	};

	public string name;
	public int physicalAttack = 0;
	public int physicalDefense = 0;
	public int techAttack = 0;
	public int techDefense = 0;
	public int advancement = 0;

	public PlayerAction(string name, int pAtk, int pDef, int tAtk, int tDef, int adv) {
		this.name = name;
		physicalAttack = pAtk;
		physicalDefense = pDef;
		techAttack = tAtk;
		techDefense = tDef;
		advancement = adv;
	}

	public PlayerAction Clone() {
		return new PlayerAction (name, physicalAttack, physicalDefense, techAttack, techDefense, advancement);
	}

	public static PlayerAction GetRandom() {
		return actions [Random.Range (0, actions.Length)].Clone();
	}
}
