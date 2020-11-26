using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class TurnManager : MonoBehaviour {

	private static TurnManager instance;

	public static Action OnTurnBegin;
	public static Action OnTurnEnd;

	private bool turnInProgress = false;
	private int turnPhaseFinishedCount = 0;

	private int enemyCount = 0;
	private int enemyDeathCount = 0;

	[SerializeField] private GameObject playerHUD;
	[SerializeField] private GameObject winScreen;
	[SerializeField] private GameObject defeatScreen;


	private void Start () {
		if (instance == null) 
			instance = this;
		else if (instance != this) {
			Destroy(gameObject);
			return;
		}

		instance.playerHUD.SetActive(true);
		instance.winScreen.SetActive(false);
		instance.defeatScreen.SetActive(false);
	}

	public static TurnManager GetInstance () {
		return instance;
	}


	public void PlayTurn () {
		if (turnInProgress)
			return;

		StartCoroutine(TurnEventHandler());
    }

	public void TurnPhaseFinished () {
		if (!instance.turnInProgress)
			return;

		instance.turnPhaseFinishedCount++;
	}

	private IEnumerator TurnEventHandler () {
		turnInProgress = true;

		// Begin turn event.
		turnPhaseFinishedCount = 0;

		OnTurnBegin();

		do
			yield return null;
		while (turnPhaseFinishedCount < OnTurnBegin.GetInvocationList().Length);

		// End turn event.
		turnPhaseFinishedCount = 0;

		OnTurnEnd();

		do
			yield return null;
		while (turnPhaseFinishedCount < OnTurnEnd.GetInvocationList().Length);


		turnInProgress = false;
	}


	public void AddEnemy () {
		Invoke("SubcribeEnemy", 0.1f);
	}

	private void SubcribeEnemy () {
		enemyCount++;
	}


	public void OnDeath (CharacterType type) {
		switch (type) {
			case CharacterType.PLAYER:
				instance.playerHUD.SetActive(false);
				instance.defeatScreen.SetActive(true);
				break;
			case CharacterType.ENEMY:
				instance.enemyDeathCount++;
				if (instance.enemyDeathCount == instance.enemyCount) {
					instance.playerHUD.SetActive(false);
					instance.winScreen.SetActive(true);
				}
				break;
		}
	}

	public void ResetLevel () {
		SceneManager.LoadScene("Test Level");
	}

	public void ExitGame () {
		Application.Quit();
	}


}