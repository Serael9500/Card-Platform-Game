using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICardAction {

    void Action (Character_Controller controller);

	void TraceAction (Character_Controller controller);


}