using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Card Actions/Defend")]
public class DefendAction : CardAction {

    public int amount;


    public override void Action (Character_Controller controller) {
        controller.ExecuteAction(controller.Defend(amount));
    }

	public override string ToString () {
		return string.Format("Add {0} armour points.\n", amount);
	}

	public override void TraceAction (Character_Controller controller) {}


}