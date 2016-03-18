using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerDashboardScript : MonoBehaviour {

	public Image advancementBar, healthBar, nameBG;
	public Image advancementLevel1, advancementLevel2, advancementLevel3;
	public Transform augmentationArea;
	public GameObject augmentation;

	public Text name_display, health_display;
	public PlayerScript player;

	public Text abilities;
	public Image abilitiesBG;

	private float health = GameManagerScript.INITIAL_HEALTH;
	private float advancement = -1;

	// 0 = hidden (not active)
	// 1 = visible
	// 2 = just the title is visible
	private int augmentationState = 0;
	private Transform augmentationDisplay;

	// Use this for initialization
	void Start () {
		health = 0;

		augmentationDisplay = augmentationArea;
	}
	
	// Update is called once per frame
	void Update () {
		if (!player)
			return;

		// Check player's health against my own and animate
		if (health != player.health) {
			health += (player.health - health) / 3.0f;

			if (Mathf.Abs (health - player.health) < 0.1)
				health = player.health;

			UpdateBar (healthBar, health_display, health, player.max_health, Vector2.zero);
		}

		if (player.school != null && advancement != player.school.advancement) {
			advancement += (player.school.advancement - advancement) / 3.0f;

			if (Mathf.Abs (advancement - player.school.advancement) < 0.1)
				advancement = player.school.advancement;

			if (advancement >= 6)
				advancementLevel1.color = Color.yellow;
			else
				advancementLevel1.color = Color.white;

			if (advancement >= 12)
				advancementLevel2.color = Color.yellow;
			else
				advancementLevel2.color = Color.white;
			
			if (advancement >= 18)
				advancementLevel3.color = Color.yellow;
			else
				advancementLevel3.color = Color.white;
			
			UpdateBar (advancementBar, null, advancement + (42 - 18), 42, new Vector2(0, 46));
		}

		if (player.augmentation == null) {
			while (augmentationDisplay != augmentationArea) {
				// "Pop" off augmentations
				Transform parent = augmentationDisplay.parent;
				Destroy (augmentationDisplay.gameObject);
				augmentationDisplay = parent;
			}
		}

		if (player.school != null && nameBG.color != player.school.color) {
			abilitiesBG.color = nameBG.color = player.school.color;

			abilities.text = "Level 1: " + player.school.levels[1].description + "\n\n" +
				"Level 2: " + player.school.levels[2].description + "\n\n" +
				"Level 3: " + player.school.levels[3].description + "\n\n";
		}
	}

	public void AddAugmentation(Card newAug) {
		GameObject newAugmentation = GameObject.Instantiate (augmentation);

		newAugmentation.GetComponent<CardDisplayScript> ().card = newAug;
//		newAugmentation.GetComponent<Image> ().color = CardDisplayManager.instance.DisplayColor (newAug.color);
		newAugmentation.transform.SetParent(augmentationDisplay);
		if (augmentationDisplay == augmentationArea)
			newAugmentation.transform.localPosition = Vector2.zero;
		else
			newAugmentation.transform.localPosition = new Vector2 (0, 160);
		newAugmentation.transform.localScale = Vector3.one;

		augmentationDisplay = newAugmentation.transform;
	}

	void UpdateBar(Image bar, Text display, float num, int max, Vector2 offset) {
		if (display != null)
			display.text = "" + Mathf.Round (num);

		bar.fillAmount = num / max;
		Vector2 bottomLeft = offset + Vector2.left * (1 - bar.fillAmount) * ((RectTransform)bar.transform).sizeDelta.x;

		((RectTransform)bar.transform).localPosition = bottomLeft;
	}
}
