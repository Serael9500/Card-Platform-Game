using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class NonUILayout : MonoBehaviour {

	[SerializeField] private float spacing = 0f;
	[SerializeField] private float padding = 0f;

	private int previousChildCount = 0;


	private void OnValidate () {
		UpdateChildren();
	}

	private void Update () {
		if (previousChildCount != transform.childCount || HasAnyChildChanged()) {
			UpdateChildren();
			previousChildCount = transform.childCount;
		}
	}

	void UpdateChildren () {
		Vector2 offset = transform.position - transform.right * (transform.lossyScale.x / 2 - padding);

		for (int i = 0; i < transform.childCount; i++) {
			Transform child = transform.GetChild(i);
			child.position = offset + (Vector2)transform.right * (child.lossyScale.x / 2);
			offset += (Vector2)transform.right * (child.lossyScale.x + spacing);
		}
	}

	private bool HasAnyChildChanged () {
		for (int i = 0; i < transform.childCount; i++)
			if (transform.GetChild(i).hasChanged)
				return true;
		return false;
	}


}
