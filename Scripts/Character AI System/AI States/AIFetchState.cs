using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFetchState : IAIState {

    public void Handle (CharacterAI characterAI, Character_Controller controller) {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(characterAI.transform.position, characterAI.detectionRadius, controller.targetMask);
        Character_Controller target = null;

        foreach (Collider2D collider in colliders) {
			
			// Check if the collider is part of a character.
			Character_Controller candidate = collider.gameObject.GetComponent<Character_Controller>();

			if (candidate == null)
				continue;
			else if (candidate == controller)
				continue;

            // Check if the candidate is at the character's "sight".
            Vector2 direction = (Vector2)(candidate.transform.position - characterAI.transform.position);

			if (controller.faceDirection != Mathf.Sign(direction.x))
                continue;

            RaycastHit2D hit = Physics2D.Raycast(characterAI.transform.position, direction.normalized, direction.magnitude, controller.obstacleMask);
            if (hit)
                continue;

			// Check if the candidate is the nearest one.
			if (target == null)
                target = candidate;
            else {
                float dst2target = Vector2.Distance(characterAI.transform.position, target.transform.position);
                float dst2candidate = Vector2.Distance(characterAI.transform.position, candidate.transform.position);

                if (dst2candidate < dst2target)
                    target = candidate; 
            }
        }
        if (target != null)
            characterAI.ChangeState(new AIChaseState(target));
    }


}