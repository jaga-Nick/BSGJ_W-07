using UnityEngine;
using InGame.Model;

namespace InGame.Presenter
{
    public class UfoPresenter : MonoBehaviour
    {
        public UfoModel ufoModel { get; private set; }
        
        private void Awake()
        {
            ufoModel = gameObject.GetComponent<UfoModel>();
        }

        private void OnDestroy()
        {
            ufoModel?.DestroyUfo(gameObject);
        }
    }
}
