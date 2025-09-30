using UnityEngine;
using Ending.Model;
using Ending.View;
using InGame.Model;

namespace Ending.Presenter
{
    /// <summary>
    /// リザルト画面のスコア表示に関するModelとViewを仲介するPresenter。
    /// </summary>
    public class ResultPresenter : MonoBehaviour
    {
        // --- 関連コンポーネント ---
        private ResultScoreView resultScoreView; 
        private ResultScoreModel resultScoreModel = new ResultScoreModel(); 
    
        /// <summary>
        /// スクリプトインスタンスがロードされたときに呼び出されます。
        /// </summary>
        private void Awake()
        {
            // 自身のGameObjectにあるViewコンポーネントを取得
            resultScoreView = GetComponent<ResultScoreView>();
            if (resultScoreView == null)
            {
                Debug.LogError("同じGameObjectにResultScoreViewが見つかりません！", this);
                enabled = false;
                return;
            }
        }


        private void Start()
        {
            // Modelに最終スコアを設定
            int targetScore = ScoreModel.Instance().score;
            resultScoreModel.SetTargetScore(targetScore);
            
            // Viewの初期表示を0に設定
            resultScoreView.UpdateScoreText(0);
        }
    

        private void Update()
        {
            // Modelの状態をチェックし、カウントアップが完了していれば処理を中断
            if (resultScoreModel.IsCountUpFinished())
            {
                return;
            }

            // Viewからカウントアップ時間を取得
            float duration = resultScoreView.GetCountUpDuration();
            
            // Modelに計算の更新を依頼
            resultScoreModel.UpdateScoreCalculation(Time.deltaTime, duration);

            // Modelから現在の表示スコアを取得
            int displayScore = resultScoreModel.GetCurrentDisplayScore();
            
            // Viewにスコア表示の更新を指示
            resultScoreView.UpdateScoreText(displayScore);
        }
    }
}