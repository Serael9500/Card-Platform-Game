using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character_Controller))]
public class CharacterAI : MonoBehaviour, IPlayTurn {

	public CharacterType characterType;

	[Header("Fetch state vars")]
	public float detectionRadius;

	[Header("Chase state vars")]
	public float jumpThreshold;
	public float attackRange {
		get {
			return Mathf.Max(meleeAttackRange, rangedAttackRange) / 2;
		}
	}
	public CardData leftMoveCard, rightMoveCard;
	public CardData jumpCard, leftJumpCard, rightJumpCard;
	public float maxChanseRange {
		get {
			return detectionRadius;
		}
	}

	[Header("Combat state vars")]
    public List<CardData> meleeAttackCards, rangedAttackCards;
	public float meleeAttackRange {
		get {
			return controller.meleeWeaponArea.x;
		}
	}
	public float rangedAttackRange {
		get {
			return controller.projectileRange;
		}
	}

	private Character_Controller controller;
	private IAIState state;

	private TurnManager turnManager;


	private void OnDrawGizmosSelected () {
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere(transform.position, detectionRadius);
	}

	private void Start () {
		controller = GetComponent<Character_Controller>();
        state = new AIFetchState();

		TurnManager.OnTurnBegin += OnTurnBegin;
		TurnManager.OnTurnEnd += OnTurnEnd;

		StartCoroutine(controller.SetCharacterType(characterType));
    }


    public void ChangeState (IAIState state) {
		Debug.Log(string.Format("{0} -> {1}", this.state.ToString(), state.ToString()));
        this.state = state;
    }

    public void OnTurnBegin () {
		//Debug.Log("State = " + state.ToString());
		state.Handle(this, controller);

		controller.PlayTurn();
	}

    public void OnTurnEnd () {
		if (turnManager == null)
			turnManager = TurnManager.GetInstance();

		turnManager.TurnPhaseFinished();
	}

	public void PlayCard (CardData cardData) {
		controller.AddCardToPlay(cardData);
	}


}