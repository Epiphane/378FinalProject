using UnityEngine;
using System.Collections;

public class ManaBarScript : MonoBehaviour {

	public static int MAX_LEVEL = 3;

	public GameObject orbPrefab;
	public Sprite orbImage;

	private SpriteRenderer[] orbs;
	private int _val;

	public int val {
		get { return _val; }
		set {
			_val = value;

			for (int i = 0; i < orbs.Length; i++) {
				orbs [i].enabled = (i < value);
			}
		}
	}

	/* Increment value and return whether to combo */
	public bool Increment () {
		val = (val + 1) % (MAX_LEVEL + 1);

		return val == 0;
	}

	// Use this for initialization
	void Start () {
		orbs = new SpriteRenderer[MAX_LEVEL];

		for (int i = 0; i < MAX_LEVEL; i++) {
			GameObject orb = GameObject.Instantiate (orbPrefab);
			orb.GetComponent<SpriteRenderer> ().sprite = orbImage;
			orb.transform.position = transform.position + new Vector3 (i * 0.5f, 0, 0);

			orbs [i] = orb.GetComponent<SpriteRenderer> ();
		}

		val = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
