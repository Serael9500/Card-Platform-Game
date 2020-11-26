using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Card Actions/Heal")]
public class HealAction : CardAction {

    public int amount;


    public override void Action (Character_Controller controller) {
        controller.ExecuteAction(controller.Heal(amount));
    }

	public override string ToString () {
		return string.Format("Heala {0} health points.\n", amount);
	}

	public override void TraceAction (Character_Controller controller) {}


}