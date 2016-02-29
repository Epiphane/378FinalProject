using UnityEngine;
using System.Collections;

public class Utils {

	public static T Find<T>(T existing, string name) {
		if (existing != null)
			return existing;

		GameObject GO = GameObject.Find (name);

		if (GO == null) {
			Debug.LogError (name + " not found!");
			return default(T);
		}

		return GO.GetComponent<T> ();
	}
}
