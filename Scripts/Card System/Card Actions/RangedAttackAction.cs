using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Card Actions/Ranged Attack")]
public class RangedAttackAction : CardAction {

    public int damage;
    public ProjectileData projectileData;


    public override void Action (Character_Controller controller) {
        controller.ExecuteAction(controller.RangedAttack(damage, projectileData));
    }

	public override string ToString () {
		return string.Format("Shoot a projectile that makes {0} damage points.\n", damage);
	}

	public override void TraceAction (Character_Controller controller) {}


}