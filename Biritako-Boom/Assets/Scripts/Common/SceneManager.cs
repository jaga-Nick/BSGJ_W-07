using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Common
{
    /// <summary>
    /// シーンマネージャーとして設計。
    /// Addressableでの非同期読み込みとしてシーンを運用します。
    /// </summary>
    public class SceneManager : SingletonMonoBehaviourBase<SceneManager>
    {
        /// <summary>
        /// メイン
        /// </summary>
        private AsyncOperationHandle<SceneInstance> _mainScene = default;
        private ISceneInfo _mainSceneInfo = null;

        /// <summary>
        /// サブ（上書きで呼び出すシーン⇒メニュー等）
        /// </summary>
        private AsyncOperationHandle<SceneInstance> _subScene = default;
        private ISceneInfo _subSceneInfo = null;


        /// <summary>
        /// 単一シーンロード:メイン
        /// </summary>
        /// <param name="mainSceneInfo"></param>
        public async UniTask LoadMainScene(ISceneInfo mainSceneInfo)
        {
            if( _mainSceneInfo != null)
            {
                //終了処理
                await _mainSceneInfo.End();
                _mainSceneInfo.InputStop();
            }
        

            //こちらでメインのロードを行う。
            using (var _cts = new CancellationTokenSource())
            {
                //トークン発行
                var token = _cts.Token;
                try
                {
                    if (_subSceneInfo != null)
                    {
                        await UnloadSubScene();
                    }

                    //ロード
                    _mainScene = Addressables.LoadSceneAsync(mainSceneInfo.SceneName,UnityEngine.SceneManagement.LoadSceneMode.Single);
                    await _mainScene.ToUniTask(cancellationToken: token);
                }
                catch (OperationCanceledException)
                {
                    Debug.Log("シーンロードキャンセル");
                }
                finally 
                {
                    //メインシーンインスタンス更新
                    _mainSceneInfo = mainSceneInfo;

                    //初期化処理(ゲーム開始等の処理）
                    _mainSceneInfo.InputStart();
                    await _mainSceneInfo.Init();
                }
            }
        }
        
        
        /// <summary>
        /// シーンロード：サブ用。（メニュー画面等）
        /// </summary>
        /// <param name="sceneInfo"></param>
        /// <returns></returns>
        public async UniTask LoadSubScene(ISceneInfo sceneInfo)
        {
            using (var cts = new CancellationTokenSource())
            {
                var token = cts.Token;
            
                try
                {
                    _subScene = Addressables.LoadSceneAsync(sceneInfo.SceneName,UnityEngine.SceneManagement.LoadSceneMode.Additive);
                    await _subScene.ToUniTask(cancellationToken: token);
                }
                catch (OperationCanceledException)
                {
                    Debug.Log("シーンロードキャンセル");
                }
            }
        }
    
        /// <summary>
        /// サブシーンをアンロードする。
        /// </summary>
        public async UniTask UnloadSubScene()
        {
            if (_subSceneInfo!=null)
            {
                //サブシーンのActionMapを終了
                _subSceneInfo?.InputStop();
                //終了処理
                await _subSceneInfo.End();

                using (var _cts = new CancellationTokenSource()) { 

                    var token = _cts.Token;
                    try
                    {
                        //アンロード
                        _subScene = Addressables.UnloadSceneAsync(_subScene);
                        await _subScene.ToUniTask(cancellationToken: token);
                    }
                    catch (OperationCanceledException)
                    {
                        Debug.Log("シーンアンロードキャンセル");
                    }
                    finally
                    {
                        _subSceneInfo = null;
                        _subScene=default;
                    }
                }
            }
        }

        /// <summary>
        /// 初期に呼び出す処理(メインシーン初期設定をここで行う）
        /// </summary>
        [RuntimeInitializeOnLoadMethod]
        public static void Init()
        {

        }
    }
}
