using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character_Controller))]
public class Player : MonoBehaviour, IPlayTurn {

	[SerializeField] private Deck deck;

	[SerializeField] private int movementCardsInHand;
	[SerializeField] private int combatCardsInHand;
	[SerializeField] private CardDropArea handArea;
	[SerializeField] private CardDropArea cardsToPlayArea;

    private Character_Controller controller;
	private TurnManager turnManager;


	private void OnValidate () {
		movementCardsInHand = Mathf.Clamp(movementCardsInHand, 0, int.MaxValue);
		combatCardsInHand = Mathf.Clamp(combatCardsInHand, 0, int.MaxValue);
	}

	private void Start () {
        controller = GetComponent<Character_Controller>();

		TurnManager.OnTurnBegin += OnTurnBegin;
		TurnManager.OnTurnEnd += OnTurnEnd;

		StartCoroutine(controller.SetCharacterType(CharacterType.PLAYER));

		// Generate first hand.
		Invoke("GenerateHand", 0.1f);
    }

	public void OnTurnBegin () {
		List<CardData> cardsToPlay = cardsToPlayArea.GetCards();

		foreach (CardData card in cardsToPlay)
			controller.AddCardToPlay(card);

		controller.PlayTurn();

		handArea.RemoveAllCards();
	}

	public void OnTurnEnd () {
		cardsToPlayArea.RemoveAllCards();

		GenerateHand();

		if (turnManager == null)
			turnManager = TurnManager.GetInstance();

		turnManager.TurnPhaseFinished();
	}

	private CardData GetRandomCard (List<CardData> cards) {
		float range = 0f;

		foreach (CardData card in cards)
			range += card.spawnPercent;

		float random = Random.Range(0f, range);
		float top = 0f;

		foreach (CardData card in cards) {
			top += card.spawnPercent;

			if (random < top)
				return card;
		}

		return null;
	}

	private void AddRandomCardSet (ref List<CardData> handCards, float total, List<CardData> cardSet) {
		int counter = 0;

		while (counter < total) {
			CardData card = GetRandomCard(cardSet);

			if (handCards.Contains(card))
				continue;

			handCards.Add(card);
			counter++;
		}
	}

	private void GenerateHand () {
		List<CardData> handCards = new List<CardData>();

		AddRandomCardSet(ref handCards, movementCardsInHand, deck.movementCards);
		AddRandomCardSet(ref handCards, combatCardsInHand, deck.combatCards);

		handArea.SetCards(handCards);
	}


	[System.Serializable]
	private struct Deck {
		public List<CardData> movementCards;
		public List<CardData> combatCards;
	}


}