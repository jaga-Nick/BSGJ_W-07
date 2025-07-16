using UnityEngine;
using Ending.Model;
using Ending.View;
using InGame.Model; // ScoreModelを参照するために必要

namespace Ending.Presenter
{
    /// <summary>
    /// リザルト画面のスコア表示に関するModelとViewを仲介するPresenter。
    /// </summary>
    public class ResultPresenter : MonoBehaviour
    {
        // --- 関連コンポーネント ---
        private ResultScoreView _resultScoreView; 
        private ResultScoreModel _resultScoreModel = new ResultScoreModel(); 
    
        /// <summary>
        /// スクリプトインスタンスがロードされたときに呼び出されます。
        /// </summary>
        private void Awake()
        {
            // 自身のGameObjectにあるViewコンポーネントを取得
            _resultScoreView = GetComponent<ResultScoreView>();
            if (_resultScoreView == null)
            {
                Debug.LogError("同じGameObjectにResultScoreViewが見つかりません！", this);
                enabled = false;
                return;
            }
        }


        private void Start()
        {
            // Modelに最終スコアを設定
            int targetScore = ScoreModel.Instance().Score;
            _resultScoreModel.SetTargetScore(targetScore);
            
            // Viewの初期表示を0に設定
            _resultScoreView.UpdateScoreText(0);
        }
    

        private void Update()
        {
            // Modelの状態をチェックし、カウントアップが完了していれば処理を中断
            if (_resultScoreModel.IsCountUpFinished())
            {
                return;
            }

            // Viewからカウントアップ時間を取得
            float duration = _resultScoreView.GetCountUpDuration();
            
            // Modelに計算の更新を依頼
            _resultScoreModel.UpdateScoreCalculation(Time.deltaTime, duration);

            // Modelから現在の表示スコアを取得
            int displayScore = _resultScoreModel.GetCurrentDisplayScore();
            
            // Viewにスコア表示の更新を指示
            _resultScoreView.UpdateScoreText(displayScore);
        }
    }
}