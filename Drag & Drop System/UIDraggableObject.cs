using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(LayoutElement), typeof(CanvasGroup))]
public class UIDraggableObject : DraggableObject {

	private LayoutElement layoutElement;
	private CanvasGroup canvasGroup;


	private void Awake () {
		canvasGroup = GetComponent<CanvasGroup>();
		layoutElement = GetComponent<LayoutElement>();
	}


	protected override void EnableCollisions () {
		canvasGroup.blocksRaycasts = true;
	}

	protected override void DisableCollisions () {
		canvasGroup.blocksRaycasts = true;
	}

	protected override void UpdatePosition (PointerEventData pointerEventData) {
		transform.position = pointerEventData.position;
	}

	protected override void CreatePlaceHolder () {
		base.CreatePlaceHolder();

		LayoutElement placeHolderLayoutElement = placeHolder.AddComponent<LayoutElement>();
		placeHolderLayoutElement.preferredWidth = layoutElement.preferredWidth;
		placeHolderLayoutElement.preferredHeight = layoutElement.preferredHeight;
		placeHolderLayoutElement.flexibleWidth = 0f;
		placeHolderLayoutElement.flexibleHeight = 0f;
	}


}
