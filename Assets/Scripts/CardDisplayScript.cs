using UnityEngine;
using System.Collections;

public class CardDisplayScript : MonoBehaviour {

	public enum Type { NONE, ATTACK, SPELL, BLOCK };
	public enum Color { BLANK, RED, BLUE, GREEN };

	private SpriteRenderer cardType;
	private SpriteRenderer cardFrame;

	private Color color = Color.BLANK;
	private Type type = Type.NONE;

	/* Frames for different color cards */
	public Sprite redFrame;
	public Sprite blueFrame;
	public Sprite greenFrame;

	/* Sprites for each type of card */
	public Sprite attack;
	public Sprite spell;
	public Sprite block;

	// Use this for initialization
	void Start () {
		cardFrame = GetComponent<SpriteRenderer>();
		cardType = transform.FindChild ("Display").GetComponent<SpriteRenderer>();

		if (color != Color.BLANK)
			SetColor (color);

		if (type != Type.NONE)
			SetType (type);
	}

	public void SetColorAndType (Color color, Type type) {
		SetColor (color);
		SetType (type);
	}

	public void SetColor (Color color) {
		this.color = color;

		if (cardFrame != null) {
			switch (color) {
			case Color.RED:
				cardFrame.sprite = redFrame;
				break;
			case Color.BLUE:
				cardFrame.sprite = blueFrame;
				break;
			case Color.GREEN:
				cardFrame.sprite = greenFrame;
				break;
			default:
				Debug.LogError ("Color not recognized: " + color);
				break;
			}
		}
	}

	public void SetType (Type type) {
		this.type = type;

		if (cardType != null) {
			switch (type) {
			case Type.ATTACK:
				cardType.sprite = attack;
				break;
			case Type.SPELL:
				cardType.sprite = spell;
				break;
			case Type.BLOCK:
				cardType.sprite = block;
				break;
			default:
				Debug.LogError ("Type not recognized: " + type);
				break;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
