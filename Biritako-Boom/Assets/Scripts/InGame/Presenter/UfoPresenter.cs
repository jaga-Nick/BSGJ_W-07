using UnityEngine;
using InGame.Model;

namespace InGame.Presenter
{
    public class UfoPresenter : MonoBehaviour
    {
        public UfoModel ufoModel { get; private set; }
        
        private void Awake()
        {
            ufoModel = new UfoModel();
            
        }

        private void OnDestroy()
        {
            ufoModel?.DestroyUfo(gameObject);
        }
    }
}
