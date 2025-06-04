using InGame.Presenter;
using UnityEngine;

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
    
    //その場に生成。
    void InstanceSocket()
    {
        Instantiate(PrefabSocket,gameObject.transform);
    }

    public void Update()
    {
        
    }
}
