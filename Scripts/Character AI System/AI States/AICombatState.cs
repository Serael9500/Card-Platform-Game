using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICombatState : IAIState {

    private Character_Controller target;


    public AICombatState (Character_Controller target) {
        this.target = target;
    }

    public void Handle (CharacterAI characterAI, Character_Controller controller) {
        Vector2 distance = (target.transform.position - characterAI.transform.position).normalized;
        bool meleeAttack = characterAI.meleeAttackCards.Count > 0 && distance.magnitude < characterAI.meleeAttackRange;
        bool rangedAttack = characterAI.rangedAttackCards.Count > 0 && distance.magnitude < characterAI.rangedAttackRange;

		//Debug.Log(string.Format("meleeAttack = {0} | rangedAttack = {1}", meleeAttack, rangedAttack));

        if (meleeAttack && rangedAttack) {
            float t = Random.value;
            meleeAttack = t < 0.5f;
            rangedAttack = t >= 0.5f;
        } else if (!meleeAttack && !rangedAttack) {
            characterAI.ChangeState(new AIChaseState(target));
            return;
        }

        Attack(characterAI, (meleeAttack) ? characterAI.meleeAttackCards : characterAI.rangedAttackCards);
    }

    private void Attack (CharacterAI characterAI, List<CardData> cards) {
        int i = Random.Range(0, cards.Count);
        characterAI.PlayCard(cards[i]);
    }

}