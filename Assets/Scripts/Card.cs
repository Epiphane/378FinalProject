using UnityEngine;

public class Card {
	public enum Type { NONE, ATTACK, SPELL, BLOCK };
	public enum Color { BLANK, RED, BLUE, GREEN };

	public static Type[] types = { Type.ATTACK, Type.SPELL, Type.BLOCK };
	public static Color[] colors = { Color.BLUE, Color.RED, Color.GREEN };

	private Type _type;
	private Color _color;
	private Stats _stats;

	public Type type { get { return _type; } }
	public Color color { get { return _color; } }
	private Stats stats { get { return _stats; } }

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
	private class Stats {
		public int physicalAttack = 0;
		public int magicalAttack = 0;
		public int physicalDef = 0;
		public int magicalDef = 0;
	}

	public void Action(Card other, PlayerScript actor, PlayerScript victim) {
		Stats otherStats = other.stats;

		victim.health -= Mathf.Max (0, stats.magicalAttack - otherStats.magicalDef)
			+ Mathf.Max (0, stats.physicalAttack - otherStats.physicalDef);
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