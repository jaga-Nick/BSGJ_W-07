using UnityEngine;
using InGame.Model;
using InGame.View;

namespace InGame.Presenter
{
    public class SocketPresenter : MonoBehaviour
    {
        private SocketModel Model;
        private SocketView View;
        private void Awake()
        {
            Model = gameObject.GetComponent<SocketModel>();
            View = gameObject.GetComponent<SocketView>();
        }


    }
}
