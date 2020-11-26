using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropArea : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {

	public virtual void OnDrop (PointerEventData pointerEventData) {
		// Check if there is a draggable object.
		DraggableObject draggableObject = pointerEventData.pointerDrag.GetComponent<DraggableObject>();
		if (draggableObject == null)
			return;

		// Change the card's drop area to this one.
		draggableObject.parentToReturn = transform;
	}

	public virtual void OnPointerEnter (PointerEventData pointerEventData) {
		// Check if anything is being dragged.
		if (pointerEventData.pointerDrag == null)
			return;

		// Check if there is a draggable object.
		DraggableObject draggableObject = pointerEventData.pointerDrag.GetComponent<DraggableObject>();

		if (draggableObject == null)
			return;

		// Change the placeHolder's area to this one.
		draggableObject.placeHolderParent = transform;
	}

	public virtual void OnPointerExit (PointerEventData pointerEventData) {
		// Check if anything is being dragged. 
		if (pointerEventData.pointerDrag == null)
			return;

		// Check if there is a draggable object.
		DraggableObject draggableObject = pointerEventData.pointerDrag.GetComponent<DraggableObject>();
		if (draggableObject == null)
			return;

		// If the placeholder's drop area is this one and the card's isn't over it, set placeHodler's one to be 
		// the same as the one of the draggable object.
		if (draggableObject.placeHolderParent == transform)
			draggableObject.placeHolderParent = draggableObject.parentToReturn;
	}


}
