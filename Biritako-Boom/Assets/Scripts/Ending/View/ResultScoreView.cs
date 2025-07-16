using UnityEngine;
using TMPro;

namespace Ending.View
{
    /// <summary>
    /// リザルト画面のスコア表示を担当するView。
    /// UI要素への参照と、表示更新メソッドのみを持つ。
    /// </summary>
    public class ResultScoreView : MonoBehaviour
    {
        [Header("UI要素")]
        [SerializeField] private TextMeshProUGUI scoreText;

        [Header("見た目の設定")]
        [SerializeField] private float countUpDuration = 3f; // カウントアップにかける時間（秒）

        // --- ゲッターメソッド ---

        /// <summary>
        /// カウントアップに要する時間を返します。
        /// </summary>
        public float GetCountUpDuration() => countUpDuration;

        // --- 表示更新メソッド ---

        /// <summary>
        /// Presenterから受け取ったスコアを元に、テキスト表示を更新します。
        /// </summary>
        /// <param name="score">表示するスコア</param>
        public void UpdateScoreText(int score)
        {
            // スコアが0の場合は空文字、それ以外は数値に変換
            string scoreStr = score == 0 ? "" : score.ToString();

            // 6桁になるように、左側をスペースで埋める
            int totalWidth = 6;
            string paddedScore = scoreStr.PadLeft(totalWidth, ' ');

            // テキストを更新
            scoreText.text = $"Score : {paddedScore}";
        }
    }
}