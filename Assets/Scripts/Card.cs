public class Card {
	public enum Type { NONE, ATTACK, SPELL, BLOCK };
	public enum Color { BLANK, RED, BLUE, GREEN };

	public static Type[] types = { Type.ATTACK, Type.SPELL, Type.BLOCK };
	public static Color[] colors = { Color.BLUE, Color.RED, Color.GREEN };

	private Type _type;
	private Color _color;

	public Card(Type type, Color color) {
		this._type = type;
		this._color = color;
	}

	public Type type {
		get { return _type; }
	}

	public Color color {
		get { return _color; }
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