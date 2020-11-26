using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Card Actions/Move")]
public class MoveAction : CardAction {

    public float amount;


    public override void Action (Character_Controller controller) {
        controller.ExecuteAction(controller.Move(amount));
    }

	public override string ToString () {
		if (amount < 0f)
			return string.Format("Move {0} units to the left.\n", -amount);
		return string.Format("Move {0} units to the right.\n", amount);
	}

	public override void TraceAction (Character_Controller controller) {
		ActionTracer.AddMoveTrajectory(Mathf.Sign(amount), Mathf.Abs(amount));
	}


}