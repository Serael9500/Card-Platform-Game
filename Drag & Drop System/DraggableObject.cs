using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public abstract class DraggableObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler {

	[HideInInspector] public Transform parentToReturn;

	protected GameObject placeHolder;
	[HideInInspector] public Transform placeHolderParent;


	[SerializeField] protected bool zoom = false;
	[SerializeField] protected float zoomScaleMultiplier = 1f;
	private bool zooming = false;

	public Action OnBeginDragEvent;
	public Action OnEndDragEvent;


	public virtual void OnBeginDrag (PointerEventData pointerEventData) {
		UndoZoom();

		// Reset roation.
		transform.rotation = Quaternion.identity;

		CreatePlaceHolder();

		// Save parent.
		parentToReturn = transform.parent;

		// Dettach from the drop area.
		transform.SetParent(parentToReturn.parent);

		// Disable collisions.
		DisableCollisions();

		OnBeginDragEvent();
	}

	public virtual void OnDrag (PointerEventData pointerEventData) {
		// Set the position of the draggable object to be the mouse position.
		UpdatePosition(pointerEventData);

		// Check if the it's not correly parented and fix it.
		if (placeHolder.transform.parent != placeHolderParent)
			placeHolder.transform.SetParent(placeHolderParent);

		// Change placeHolder position in the drop area.
		int siblingIndex = placeHolderParent.childCount;

		for (int i = 0; i < parentToReturn.childCount; i++) {
			Transform child = parentToReturn.GetChild(i);

			if (transform.position.x < child.position.x) {
				siblingIndex = i;

				if (placeHolder.transform.GetSiblingIndex() < siblingIndex)
					siblingIndex--;

				break;
			}
		}

		placeHolder.transform.SetSiblingIndex(siblingIndex);
	}

	public virtual void OnEndDrag (PointerEventData pointerEventData) {
		// Attach the draggable object to the correct drop area
		transform.SetParent(parentToReturn);
		transform.SetSiblingIndex(placeHolder.transform.GetSiblingIndex());

		Destroy(placeHolder);

		// Enable collisions.
		EnableCollisions();

		// Match the rotation with the parent's one.
		transform.rotation = Quaternion.FromToRotation(transform.up, parentToReturn.up);

		OnEndDragEvent();
	}


	public virtual void OnPointerEnter (PointerEventData pointerEventData) {
		// Check if anything is being dragged.
		if (pointerEventData.pointerDrag != null)
			return;

		ApplyZoom();
	}

	public virtual void OnPointerExit (PointerEventData pointerEventData) {
		// Check if anything is being dragged.
		if (pointerEventData.pointerDrag != null)
			return;

		UndoZoom();
	}


	protected virtual void CreatePlaceHolder () {
		placeHolder = new GameObject(name + " Place Holder");

		placeHolder.transform.SetParent(transform.parent);
		placeHolder.transform.SetSiblingIndex(transform.GetSiblingIndex());

		placeHolderParent = transform.parent;
	}

	protected virtual void ApplyZoom () {
		if (!zoom || zooming)
			return;

		zooming = true;
		transform.localScale *= zoomScaleMultiplier;
	}

	protected virtual void UndoZoom () {
		if (!zoom || !zooming)
			return;

		zooming = false;
		transform.localScale /= zoomScaleMultiplier;
	}

	protected abstract void EnableCollisions ();
	protected abstract void DisableCollisions ();
	protected abstract void UpdatePosition (PointerEventData pointerEventData);

}
