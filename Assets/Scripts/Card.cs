using UnityEngine;

public class Card {
	public enum Type { NONE, ATTACK, SPELL, BLOCK };
	public enum Color { BLANK, RED, BLUE, GREEN };
	public enum Special { None, Counter, Double, SeeOpponentHand, ComboBreak, SunderArmor, Heal, FutureShield };

	public static Type[] types = { Type.ATTACK, Type.SPELL, Type.BLOCK };
	public static Color[] colors = { Color.BLUE, Color.RED, Color.GREEN };

	private Type _type;
	private Color _color;
	private Stats _stats;
	private Special special = Special.None;

	public Type type { get { return _type; } }
	public Color color { get { return _color; } }
	public Stats stats { get { return _stats; } }
	public Special GetSpecial() { return special; }

	public Card(Type type, Color color) {
		this._type = type;
		this._color = color;

		this._stats = new Stats ();

		switch (type) {
		case Type.ATTACK:
			_stats.physicalAttack = 2;
			break;
		case Type.SPELL:
			_stats.magicalAttack = 1;
			break;
		case Type.BLOCK:
			_stats.physicalDef = 16;
			break;
		}
	}

	/* Combat information */
	public class Stats {
		public int physicalAttack = 0;
		public int magicalAttack = 0;
		public int physicalDef = 0;
		public int magicalDef = 0;
	}

	public void Combo(ManaManagerScript manaManager) {
		bool combo = manaManager.Increment (color);

		// Do combo things!
		if (combo) {
			if (color == Color.RED) {
				if (type == Type.ATTACK) {
					stats.physicalAttack *= 3;
				} else if (type == Type.SPELL) {
					stats.magicalAttack = 3;
				} else if (type == Type.BLOCK) {
					special = Special.Counter;
				}
			} else if (color == Color.GREEN) {
				if (type == Type.ATTACK) {
					special = Special.SunderArmor;
				} else if (type == Type.SPELL) {
					special = Special.Heal;
				} else if (type == Type.BLOCK) {
					special = Special.FutureShield;
				}
			} else if (color == Color.BLUE) {
				if (type == Type.ATTACK) {
					special = Special.Double;
				} else if (type == Type.SPELL) {
					special = Special.SeeOpponentHand;
				} else if (type == Type.BLOCK) {
					special = Special.ComboBreak;
				}
			}
		}
	}

	public void Action(Card other, PlayerScript actor, PlayerScript victim) {
		Stats otherStats = other.stats;

		int damage = Mathf.Max (0, stats.magicalAttack - otherStats.magicalDef)
		             + Mathf.Max (0, stats.physicalAttack - otherStats.physicalDef);

		// Calculate normal attack
		if (other.special == Special.Counter) {
			actor.health -= 2 * stats.physicalAttack;
		} else {
			victim.health -= damage;
		}

		// Instant special effects
		if (special == Special.Double) {
			victim.health -= (stats.magicalAttack + stats.physicalAttack);
		} else if (special == Special.Heal) {
			actor.health += 2;
		}
	}

	public override string ToString () {
		string t = "Typeless", c = "Colorless";

		switch (type) {
		case Type.ATTACK:
			t = "Attack";
			break;
		case Type.SPELL:
			t = "Spell";
			break;
		case Type.BLOCK:
			t = "Block";
			break;
		}

		switch (color) {
		case Color.RED:
			c = "Red";
			break;
		case Color.BLUE:
			c = "Blue";
			break;
		case Color.GREEN:
			c = "Green";
			break;
		}

		return t + " " + c;
	}
}