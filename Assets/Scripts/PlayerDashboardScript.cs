using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerDashboardScript : MonoBehaviour {

	public Image advancementBar, healthBar;
	public Image advancementLevel1, advancementLevel2, advancementLevel3;
	public GameObject augmentation;

	public Text name_display, health_display;
	public PlayerScript player;

	private float health = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		// Check player's health against my own and animate
		if (health != player.health) {
			health += (player.health - health) / 3.0f;

			if (Mathf.Abs (health - player.health) < 0.1)
				health = player.health;

			health_display.text = "" + Mathf.Round (health);

			healthBar.fillAmount = (float) health / player.max_health;
			Vector2 bottomLeft = Vector2.left * (1 - healthBar.fillAmount) * ((RectTransform)healthBar.transform).sizeDelta.x;

			((RectTransform)healthBar.transform).offsetMax = bottomLeft + ((RectTransform)healthBar.transform).sizeDelta;
			((RectTransform)healthBar.transform).offsetMin = bottomLeft;
		}
	}
}
