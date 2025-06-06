// ResultScoreView.cs
using UnityEngine;
using TMPro; // TextMeshProUGUIを使用
using System.Collections.Generic;
using Cysharp.Threading.Tasks; // UniTaskを使用
using System.Linq; // 文字列操作に使う可能性あり
using System.Text; // StringBuilderを使用

namespace Result.View
{
    public class ResultScoreView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private float _digitDisplayDelay = 0.1f; // 各桁の表示遅延時間
        [SerializeField] private int _maxDisplayDigits = 10; // 最大表示桁数（バッファ）

        private const string SCORE_PREFIX = "Score : "; // スコアの接頭辞

        /// <summary>
        /// Viewの初期化（Presenterから呼ばれることを想定）
        /// </summary>
        public void Initialize()
        {
            if (_scoreText != null)
            {
                // 最大桁数分のスペースで初期化
                // String.Format("{0," + _maxDisplayDigits + "}", "") で右揃えの空文字列を作成
                _scoreText.text = SCORE_PREFIX + string.Empty.PadLeft(_maxDisplayDigits, ' '); // スペースで埋める
            }
            else
            {
                Debug.LogError("TextMeshProUGUI for score display is not assigned in ResultScoreView.");
            }
        }

        /// <summary>
        /// スコアを非同期で、下一桁目から順番に表示する
        /// </summary>
        /// <param name="score"></param>
        public async UniTask DisplaySingleScoreAnimation(int score)
        {
            // まずはバッファ付きの初期状態にリセット
            Initialize();

            string scoreString = score.ToString(); // スコアを文字列に変換

            // スコアが0の場合や、特定の条件での表示
            if (scoreString.Length == 0 || score == 0)
            {
                _scoreText.text = SCORE_PREFIX + string.Format("{0," + _maxDisplayDigits + "}", "0"); // ゼロを右揃えで表示
                return;
            }

            // 表示するスコア文字列の長さを、最大表示桁数に合わせる
            string paddedScoreString = scoreString.PadLeft(_maxDisplayDigits, ' ');

            // 文字列を反転させる (下一桁から処理するため)
            char[] scoreChars = paddedScoreString.ToCharArray();
            System.Array.Reverse(scoreChars); 

            string currentDisplay = ""; // 現在表示されているスコア部分

            // 各桁を順番に表示していく
            for (int i = 0; i < scoreChars.Length; i++)
            {
                // 反転しているので、scoreChars[i] は下一桁目から順にアクセスされる
                // これを currentDisplay の先頭に追加していく
                currentDisplay = scoreChars[i] + currentDisplay;

                // スコア部分の文字列を、バッファを考慮して整形
                string formattedScorePart = currentDisplay.PadLeft(_maxDisplayDigits, ' ');
                if (formattedScorePart.Length > _maxDisplayDigits)
                {
                    // _maxDisplayDigitsを超える部分は切り捨て（または、最大桁数を超えた場合の表示方法を検討）
                    formattedScorePart = formattedScorePart.Substring(formattedScorePart.Length - _maxDisplayDigits);
                }
                
                // TextMeshProUGUIにセット
                _scoreText.text = SCORE_PREFIX + formattedScorePart;

                // 各桁が表示されるまでの遅延
                await UniTask.Delay(Mathf.RoundToInt(_digitDisplayDelay * 1000));
            }
        }
    }
}