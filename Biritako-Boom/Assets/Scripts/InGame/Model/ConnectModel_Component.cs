using Common;
using InGame.Presenter;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// コンポーネント指向的テスト
/// </summary>
public class ConnectModel_Component : MonoBehaviour
{
    //呼び出し用。
    [SerializeField]
    private GameObject PrefabSocket;
    //ソケットデータ
    private SocketPresenter SocketPresenter;

    private InputSystem_Actions tes;
    //その場に生成。
    void InstanceSocket()
    {
        
        Instantiate(PrefabSocket,gameObject.transform.position,Quaternion.identity);
    }
    private void Awake()
    {
        InputSystemActionsManager a = InputSystemActionsManager.Instance();
        tes = a.GetInputSystem_Actions();
        a.PlayerEnable();

    }
    public void Update()
    {
        if (tes.Player.Jump.WasPressedThisFrame()) 
        {
            Debug.Log("a");
            InstanceSocket();        
        }
    }
}
