using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Card Actions/Jump")]
public class JumpAction : CardAction {

	public float height;
	public float horizontalAmount;


    public override void Action (Character_Controller controller) {
		Vector2 velocity;
		float t_up, t_down;
		float g_up, g_down;

		Calculate(controller, out velocity, out t_up, out t_down, out g_up, out g_down);

		controller.ExecuteAction(controller.Jump(velocity));
    }

	public override string ToString () {
		if (horizontalAmount < 0f)
			return string.Format("Jump {0} units up and move {1} units to the left.\n", height, -horizontalAmount);
		else if (horizontalAmount > 0f)
			return string.Format("Jump {0} units up and move {1} units to the right.\n", height, horizontalAmount);
		return string.Format("Jump {0} units up.\n", height);
	}

	public override void TraceAction (Character_Controller controller) {
		Vector2 velocity;
		float t_up, t_down;
		float g_up, g_down;

		Calculate(controller, out velocity, out t_up, out t_down, out g_up, out g_down);
		
		ActionTracer.AddJumpTrajectory(velocity, t_up, t_down, g_up, g_down);
	}


	private void Calculate (Character_Controller controller, out Vector2 velocity, out float t_up, out float t_down, out float g_up, out float g_down) {
		/*
		 * OLD
		// y = (vy0 - vy) * t / 2	->	vy0 = sqrt(4 * g * y)
		// vy = vy0 * - 2* g * t	->	t = vy0 / (2 * g)
		// x = vx * t	->	vx = x / t
		float g = Mathf.Abs(Physics2D.gravity.y);
		float vy = Mathf.Sqrt(4 * g * height);
		float t = vy / (2 * g);
		float vx = horizontalAmount / t;
		*/

		g_up = Physics2D.gravity.y;
		g_down = Physics.gravity.y * controller.fallingGravityScale;

		t_up = Mathf.Sqrt(-2 * height / g_up);
		t_down = Mathf.Sqrt(-2 * height / g_down);

		velocity = new Vector2(
			horizontalAmount / (t_up + t_down),
			Mathf.Sqrt(-2 * Physics.gravity.y * height)
		);
	}

}