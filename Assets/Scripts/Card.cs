public class Card {
	public enum Type { NONE, ATTACK, SPELL, BLOCK };
	public enum Color { BLANK, RED, BLUE, GREEN };

	private Type type;
	private Color color;

	public Card(Type type, Color color) {
		this.type = type;
		this.color = color;
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

	public Type GetType() { return type; }
	public Color GetColor() { return color; }
}