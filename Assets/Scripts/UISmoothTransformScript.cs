using UnityEngine;
using System.Collections;

/**
 * Simple transformation component that allows you to move a GameObject
 * from point A to point B smoothly. You can either call:
 * 
 * component.MoveTo(destination, time)
 * 
 * or
 * 
 * component.Record();
 *   // do transformation changes...
 * component.StartTransition(time);
 */
public class UISmoothTransformScript : MonoBehaviour {

	private Vector3 startPos, endPos;
	private Quaternion startRot, endRot;
	private float timeElapsed, transition;
	
	// Update is called once per frame
	void Update () {
		if (timeElapsed < transition) {
			timeElapsed += Time.deltaTime;

			if (timeElapsed > transition)
				timeElapsed = transition;

//			if (transform is RectTransform) {
//				RectTransform rTransform = (RectTransform)transform;
//				rTransform.offsetMin = 
//				((RectTransform)healthBar.transform).offsetMax = bottomLeft + ((RectTransform)healthBar.transform).sizeDelta;
//				((RectTransform)healthBar.transform).offsetMin = bottomLeft;
//			} else {
				transform.position = Vector3.Lerp (startPos, endPos, timeElapsed / transition);
				transform.rotation = Quaternion.Slerp (startRot, endRot, timeElapsed / transition);
//			}
		}
	}

	public void MoveTo(Vector3 destination) {
		MoveTo (destination, 1);
	}

	public void MoveTo(Vector3 destination, float time) {
		timeElapsed = 0;
		transition = time;

		startPos = transform.position;
		endPos = destination;
	}

	public void Record() {
		timeElapsed = 0;
		startPos = transform.position;
		startRot = transform.rotation;
	}

	public void StartTransition(float time) {
		transition = time;
		endPos = transform.position;
		endRot = transform.rotation;

		transform.position = startPos;
		transform.rotation = startRot;
	}
}
