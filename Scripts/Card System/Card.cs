using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Card : MonoBehaviour {

	[HideInInspector] public CardData cardData;

	private Object background;
	private Object title;
	private Object image;
	private Object description;

	private DraggableObject draggableObject;
	private bool uiMode;


	[System.Diagnostics.Conditional("UNITY_EDITOR")]
	public void Reset () {
		UIDraggableObject uiDraggableObjetc = GetComponent<UIDraggableObject>();
		NonUIDraggableObject nonUIDraggableObject = GetComponent<NonUIDraggableObject>();

		if (uiDraggableObjetc == null && nonUIDraggableObject == null) {
			uiMode = UnityEditor.EditorUtility.DisplayDialog("Choose a Component", "You are missing one of the required componets. Please choose one to add", "UIDraggableObject", "NonUIDraggableObject");
			if (uiMode) {
				draggableObject = gameObject.AddComponent<UIDraggableObject>();
			} else {
				draggableObject = gameObject.AddComponent<NonUIDraggableObject>();
			}
		}
	}

	private void Start () {
		if (uiMode) {
			background = transform.Find("Background").GetComponent<Image>();
			title = transform.Find("Title").GetComponent<TextMeshProUGUI>();
			image = transform.Find("Image").GetComponent<Image>();
			description = transform.Find("Description").GetComponent<TextMeshProUGUI>();
		} else {
			background = transform.Find("Background").GetComponent<SpriteRenderer>();
			title = transform.Find("Title").GetComponent<TextMeshPro>();
			image = transform.Find("Image").GetComponent<SpriteRenderer>();
			description = transform.Find("Description").GetComponent<TextMeshPro>();
		}

		draggableObject.OnBeginDragEvent += RemoveCardFromCardDropArea;
		draggableObject.OnEndDragEvent += UpdateVisuals;
	}

	private void RemoveCardFromCardDropArea () {
		CardDropArea cardDropArea = draggableObject.parentToReturn.GetComponent<CardDropArea>();
		if (cardDropArea == null)
			return;

		cardDropArea.RemoveCard(this);

		UpdateVisuals();
	}


	public void UpdateVisuals () {
		StartCoroutine(DelayedUpdateVisuals());
	}

	private IEnumerator DelayedUpdateVisuals () {
		while (background == null || title == null || image == null || description == null)
			yield return null;

		// Change the visuals depending on the card drop area type.
		CardDropArea cardDropArea = transform.parent.GetComponent<CardDropArea>();

		if (cardDropArea != null)
			if (cardDropArea.type == CardDropArea.CardDropAreaType.HAND_AREA)
				ShowCardVisuals();  // Card mode.
			else
				ShowIconVisuals();  // Icon mode.
		else
			ShowCardVisuals();		// By default -> Card mode.
	}

	private void ShowCardVisuals () {
		if (uiMode) {
			((Image)background).sprite = cardData.card_background;
			((TextMeshProUGUI)title).text = cardData.name;
			((Image)image).sprite = cardData.card_image;
			((TextMeshProUGUI)description).text = cardData.card_description;
		} else {
			((SpriteRenderer)background).sprite = cardData.card_background;
			((TextMeshPro)title).text = cardData.name;
			((SpriteRenderer)image).sprite = cardData.card_image;
			((TextMeshPro)description).text = cardData.card_description;
		}
	}

	private void ShowIconVisuals () {
		if (uiMode) {
			((Image)background).sprite = cardData.icon_background;
			((Image)image).sprite = cardData.icon_image;
		} else {
			((SpriteRenderer)background).sprite = cardData.icon_background;
			((SpriteRenderer)image).sprite = cardData.icon_image;
		}
	}

	public void TraceActions (Character_Controller controller) {
		foreach (CardAction action in cardData.actions)
			action.TraceAction(controller);
	}


}
