using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerDashboardScript : MonoBehaviour {

	public Image advancementBar, healthBar;
	public Image advancementLevel1, advancementLevel2, advancementLevel3;
	public GameObject augmentation;

	public Text name_display, health_display;
	public PlayerScript player;

	private float health = GameManagerScript.INITIAL_HEALTH;
	private float advancement = -1;

	// 0 = hidden (not active)
	// 1 = visible
	// 2 = just the title is visible
	private int augmentationState = 0;

	// Use this for initialization
	void Start () {
	
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

		if (advancement != player.advancement) {
			advancement += (player.advancement - advancement) / 3.0f;

			if (Mathf.Abs (advancement - player.advancement) < 0.1)
				advancement = player.advancement;

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

		if (augmentationState == 0 && player.augmentation != null) {
			augmentationState = 1;
			augmentation.GetComponent<CardDisplayScript> ().card = player.augmentation;

			((RectTransform)augmentation.transform).localPosition = new Vector2 (5, 73);
		} else if (augmentationState > 0 && player.augmentation == null) {
			augmentationState = 0;

			((RectTransform)augmentation.transform).localPosition = new Vector2 (5, -45);
		}
	}

	void UpdateBar(Image bar, Text display, float num, int max, Vector2 offset) {
		if (display != null)
			display.text = "" + Mathf.Round (num);

		bar.fillAmount = num / max;
		Vector2 bottomLeft = offset + Vector2.left * (1 - bar.fillAmount) * ((RectTransform)bar.transform).sizeDelta.x;

		((RectTransform)bar.transform).localPosition = bottomLeft;
	}
}
