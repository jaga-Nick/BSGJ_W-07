using UnityEngine;

namespace Ending.Model
{
    /// <summary>
    /// リザルト画面のスコアに関するデータと計算ロジックを管理するModel。
    /// </summary>
    public class ResultScoreModel
    {
        // --- 内部状態 ---
        private int _targetScore;
        private float _currentScoreValue;
        private float _timer;
        private bool _isCountUpFinished;

        // --- ゲッターメソッド ---

        /// <summary>
        /// 現在表示すべきスコアの整数値を返します。
        /// </summary>
        public int GetCurrentDisplayScore() => Mathf.FloorToInt(_currentScoreValue);

        /// <summary>
        /// カウントアップが完了したかどうかを返します。
        /// </summary>
        public bool IsCountUpFinished() => _isCountUpFinished;
        
        // --- セッター/初期化メソッド ---

        /// <summary>
        /// 目標となる最終スコアを設定します。
        /// </summary>
        /// <param name="score">最終スコア</param>
        public void SetTargetScore(int score)
        {
            _targetScore = score;
            // デバッグ用にスコアが0の場合の値を設定
            if (_targetScore == 0)
            {
                _targetScore = 12345;
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
            if (_isCountUpFinished) return;

            // タイマーを進める
            _timer += deltaTime;

            // カウントアップの進捗を計算 (0.0～1.0の範囲)
            float progress = Mathf.Clamp01(_timer / duration);

            // 線形補間を使って、現在の表示スコアを計算
            _currentScoreValue = Mathf.Lerp(0, _targetScore, progress);

            // 進捗が100%に達したら、カウントアップを完了とする
            if (progress >= 1f)
            {
                _isCountUpFinished = true;
                _currentScoreValue = _targetScore; // 誤差をなくすため、最終値を強制的に設定
            }
        }
    }
}