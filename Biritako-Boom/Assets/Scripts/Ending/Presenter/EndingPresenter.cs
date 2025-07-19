using Cysharp.Threading.Tasks;
using Ending.Model;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

namespace Ending.Presenter
{
    public class EndingPresenter : MonoBehaviour
    {
        
        private VideoPlayer _vp;
        
        [Header("背景Image"), SerializeField] 
        private Image _image;

        [Header("背景スライド画像"), SerializeField] 
        private Sprite[] EndingSprites;

        [Header("リザルトのUI"), SerializeField]
        private GameObject resultUI;


        EndingEvent _endingEvent =  new EndingEvent();
        [SerializeField, Header("待機時間")] private float delayTime = 10f;
        async　void Start()
        {
            
            resultUI.SetActive(false);
            if(_vp == null)
            {
                await UniTask.Delay(System.TimeSpan.FromSeconds(delayTime));
                resultUI.SetActive(true);
                //_endingEvent.OnResultLoder();
            }
            else
            {
                _vp.loopPointReached += LoopPointReached;
                _vp.Play();
            }
        }

        public void LoopPointReached(VideoPlayer vp)
        {
            //_endingEvent.OnResultLoder();
        }
    }
}
