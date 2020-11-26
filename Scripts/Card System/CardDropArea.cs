using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDropArea : DropArea {

	[SerializeField] private GameObject cardPrefab;
	private List<Card> cards = new List<Card>();

	public CardDropAreaType type { get;  private set; }
	[SerializeField] private bool traceCardActions = false;
	[SerializeField] private Character_Controller playerController;


	private void OnValidate () {
		if (traceCardActions && playerController == null)
			Debug.LogError("playerController cannot be null");
	}

	public override void OnDrop (PointerEventData eventData) {
		base.OnDrop(eventData);

		// Check if the draggable object it's a card.
		Card card = eventData.pointerDrag.GetComponent<Card>();
		if (card == null)
			return;

		cards.Add(card);

		// Trace card actions
		TraceCardActions();
	}


	public void SetCards (List<CardData> cards) {
		RemoveAllCards();

		// Create the rest by cloning the first one
		for (int i = 0; i < cards.Count; i++) {
			Card card = Instantiate(cardPrefab, transform).GetComponent<Card>();
			card.cardData = cards[i];
			card.UpdateVisuals();
			this.cards.Add(card);
		}

		// Trace cards actions.
		TraceCardActions();
	}

	public List<CardData> GetCards () {
		List<CardData> cards = new List<CardData>();

		foreach (Card card in this.cards)
			cards.Add(card.cardData);

		return cards;
	}

	public void RemoveCard (Card card) {
		if (!cards.Contains(card) || card == null)
			return;

		int index = Mathf.Clamp(cards.IndexOf(card), 0, transform.childCount);
		Transform child = transform.GetChild(index);

		child.SetParent(null);

		cards.Remove(card);

		// Trace card actions
		TraceCardActions();
	}

	public void RemoveAllCards () {
		cards.Clear();

		while (transform.childCount > 0) {
			Transform child = transform.GetChild(0);
			child.SetParent(null);
			Destroy(child.gameObject);
		}

		cards.Clear();

		// Clear ActionTracer.
		if (traceCardActions)
			ActionTracer.Clear();
	}


	private void TraceCardActions () {
		if (!traceCardActions)
			return;

		ActionTracer.Clear();

		foreach (Card card in cards)
			card.TraceActions(playerController);
	}


	public enum CardDropAreaType {
		HAND_AREA, PLAY_AREA
	}


}
