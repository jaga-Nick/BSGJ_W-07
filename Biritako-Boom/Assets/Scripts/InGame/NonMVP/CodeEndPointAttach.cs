using InGame.NonMVP;
using UnityEngine;

public class CodeEndPointAttach : MonoBehaviour
{

    public CodeSimulater CodeSimulater { get; private set; } = null;

    public void SetCodeSimulater(CodeSimulater codeSimulater)
    { 
        CodeSimulater = codeSimulater;    
    }
}
