using UnityEngine;
using InGame.Model;
using InGame.View;
using UnityEngine.Serialization;

namespace InGame.Presenter
{
    /// <summary>
    /// コンセントのPresenter
    /// </summary>
    public class SocketPresenter : MonoBehaviour
    {
        //private SocketModel model;
        private SocketView _view;
        
        public Transform socketTipTransform;
        
        private void Awake()
        {
            //model = gameObject.GetComponent<SocketModel>();
            _view = gameObject.GetComponent<SocketView>();
            
            if (socketTipTransform == null)
            {
                socketTipTransform = transform;
            }
        }
    }
}
