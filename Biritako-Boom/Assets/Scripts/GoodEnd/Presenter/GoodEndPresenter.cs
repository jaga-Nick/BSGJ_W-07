using Cysharp.Threading.Tasks;
using GoodEnd.Model;
using UnityEngine;

namespace GoodEnd.Presenter
{
    public class GoodEndPresenter : MonoBehaviour
    { 
        GoodEndLoder goodEndLoder =  new GoodEndLoder();
        async　void Start()
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(5));// 5秒待機
            goodEndLoder.OnResultLoder();
        }
    }
}
