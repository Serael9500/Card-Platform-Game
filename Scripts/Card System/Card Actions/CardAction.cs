using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardAction : ScriptableObject, ICardAction {

	public abstract void Action (Character_Controller controller);

	public abstract void TraceAction (Character_Controller controller);


}
