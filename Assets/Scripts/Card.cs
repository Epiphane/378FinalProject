public class Card {
	public enum Type { NONE, ATTACK, SPELL, BLOCK };
	public enum Color { BLANK, RED, BLUE, GREEN };

	private Type type;
	private Color color;

	public Card(Type type, Color color) {
		this.type = type;
		this.color = color;
	}

	public Type GetType() { return type; }
	public Color GetColor() { return color; }
}