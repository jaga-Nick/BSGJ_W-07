using UnityEngine;

namespace Ending.Model
{
    /// <summary>
    /// リザルト画面のスコアに関するデータと計算ロジックを管理するModel。
    /// </summary>
    public class ResultScoreModel
    {
        // --- 内部状態 ---
        private int targetScore;
        private float currentScoreValue;
        private float timer;
        private bool isCountUpFinished;

        // --- ゲッターメソッド ---

        /// <summary>
        /// 現在表示すべきスコアの整数値を返します。
        /// </summary>
        public int GetCurrentDisplayScore() => Mathf.FloorToInt(currentScoreValue);

        /// <summary>
        /// カウントアップが完了したかどうかを返します。
        /// </summary>
        public bool IsCountUpFinished() => isCountUpFinished;
        
        // --- セッター/初期化メソッド ---

        /// <summary>
        /// 目標となる最終スコアを設定します。
        /// </summary>
        /// <param name="score">最終スコア</param>
        public void SetTargetScore(int score)
        {
            targetScore = score;
            // デバッグ用にスコアが0の場合の値を設定
            if (targetScore == 0)
            {
                targetScore = 12345;
            }
        }

        // --- 計算ロジック ---

        /// <summary>
        /// 経過時間に基づいて、現在の表示スコアを更新します。
        /// </summary>
        /// <param name="deltaTime">フレーム間の経過時間 (Time.deltaTime)</param>
        /// <param name="duration">カウントアップに要する時間</param>
        public void UpdateScoreCalculation(float deltaTime, float duration)
        {
            // 既にカウントアップが完了していれば何もしない
            if (isCountUpFinished) return;

            // タイマーを進める
            timer += deltaTime;

            // カウントアップの進捗を計算 (0.0～1.0の範囲)
            float progress = Mathf.Clamp01(timer / duration);

            // 線形補間を使って、現在の表示スコアを計算
            currentScoreValue = Mathf.Lerp(0, targetScore, progress);

            // 進捗が100%に達したら、カウントアップを完了とする
            if (progress >= 1f)
            {
                isCountUpFinished = true;
                currentScoreValue = targetScore; // 誤差をなくすため、最終値を強制的に設定
            }
        }
    }
}