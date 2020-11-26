using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAIState {
    
    void Handle (CharacterAI characterAI, Character_Controller controller);


}