using Common;
using InGame;
using UnityEngine;

/// <summary>
/// Inputを制御
/// </summary>
public class Player_Controller : MonoBehaviour
{
    private Player_Model Model;
    private InputSystem_Actions ActionMap;

    public void Awake()
    {
        ActionMap = InputSystemActionsManager.Instance().GetInputSystem_Actions(); 
    }

    public void Update()
    {
        //Model.PlayerMove(ActionMap);
    }
}
