using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIChaseState : IAIState {

    private Character_Controller target;


    public AIChaseState (Character_Controller target) {
        this.target = target;
    }

    public void Handle (CharacterAI characterAI, Character_Controller controller) {
        Vector2 distance = (target.transform.position - characterAI.transform.position).normalized;

        if (distance.magnitude <= characterAI.attackRange) {
            characterAI.ChangeState(new AICombatState(target));
            return;
        } else if (distance.magnitude >= characterAI.maxChanseRange) {
            characterAI.ChangeState(new AIFetchState());
            return;
        }

        float directionX = Mathf.Sign(distance.x);
        float directionY = Mathf.Sign(distance.y);

        if (directionY > 0f && Mathf.Abs(distance.y) >= characterAI.jumpThreshold) {
            if (directionX < 0f)
                characterAI.PlayCard(characterAI.leftJumpCard);
            else if (directionX > 0f)
                characterAI.PlayCard(characterAI.rightJumpCard);
            else
                characterAI.PlayCard(characterAI.jumpCard);
        } else {
            if (directionX < 0f)
                characterAI.PlayCard(characterAI.leftMoveCard);
            else if (directionX > 0f)
                characterAI.PlayCard(characterAI.rightMoveCard);
        }
    }


}