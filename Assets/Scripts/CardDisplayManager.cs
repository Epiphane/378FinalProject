using UnityEngine;
using System.Collections;

/* Inspiration: http://wiki.unity3d.com/index.php/AManagerClass */
/* Hangs onto the Sprite references for each image so we can swap them out easily */
public class CardDisplayManager : MonoBehaviour {
	/* Frames for different color cards */
	public Sprite blankFrame;
	public Sprite redFrame;
	public Sprite blueFrame;
	public Sprite greenFrame;

	/* Sprites for each type of card */
	public Sprite attack;
	public Sprite spell;
	public Sprite block;

	private static CardDisplayManager s_Instance = null;

	public static CardDisplayManager instance {
		get {
			if (s_Instance == null) {
				// This is where the magic happens.
				//  FindObjectOfType(...) returns the first AManager object in the scene.
				s_Instance = FindObjectOfType(typeof (CardDisplayManager)) as CardDisplayManager;
			}

			// If it is still null, create a new instance
			if (s_Instance == null) {
				GameObject obj = new GameObject("CardDisplayManager");
				s_Instance = obj.AddComponent(typeof (CardDisplayManager)) as CardDisplayManager;
				Debug.Log ("Could not locate an CardDisplayManager object. / CardDisplayManager was Generated Automaticly.");
			}

			return s_Instance;
		}
	}

	// Ensure that the instance is destroyed when the game is stopped in the editor.
	void OnApplicationQuit() {
		s_Instance = null;
	}

	public Color DisplayColor (Card.Color color) {
		switch (color) {
		case Card.Color.BLANK:
			return Color.white;
		case Card.Color.RED:
			return Color.red;
		case Card.Color.BLUE:
			return new Color(0.5f, 0.5f, 1);
		case Card.Color.GREEN:
			return Color.green;
		default:
			Debug.LogError ("Card color not found: " + color);
			return Color.white;
		}
	}
}