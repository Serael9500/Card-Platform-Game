using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider2D))]
public class NonUIDraggableObject : DraggableObject {

	new protected BoxCollider2D collider;


	protected void Awake () {
		collider = GetComponent<BoxCollider2D>();
	}


	protected override void EnableCollisions () {
		collider.enabled = true;
	}

	protected override void DisableCollisions () {
		collider.enabled = false;
	}

	protected override void UpdatePosition (PointerEventData pointerEventData) {
		transform.position = (Vector2)Camera.main.ScreenToWorldPoint(pointerEventData.position);
	}

	protected override void CreatePlaceHolder () {
		base.CreatePlaceHolder();

		placeHolder.transform.localScale = transform.localScale;
	}


}
