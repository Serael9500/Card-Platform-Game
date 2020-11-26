using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Card Actions/Melee Attack")]
public class MeleeAttackAction : CardAction {

    public int damage;


    public override void Action (Character_Controller controller) {
        controller.ExecuteAction(controller.MeleeAttack(damage));
    }

	public override string ToString () {
		return string.Format("Attack an adyacent enemy dealing {0} damage points.\n", damage);
	}

	public override void TraceAction (Character_Controller controller) {}


}