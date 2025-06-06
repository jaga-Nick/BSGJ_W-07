using Cysharp.Threading.Tasks;
using Ending.Model;
using UnityEngine;

namespace Ending.Presenter
{
    public class EndingPresenter : MonoBehaviour
    { 
        EndingEvent _endingEvent =  new EndingEvent();
        [SerializeField, Header("待機時間")] private float delayTime;
        async　void Start()
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(delayTime));
            _endingEvent.OnResultLoder();
        }
    }
}
