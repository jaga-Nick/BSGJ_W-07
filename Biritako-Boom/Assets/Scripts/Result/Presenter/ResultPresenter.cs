using UnityEngine;
using Cysharp.Threading.Tasks;
using Result.Model;
using Result.View;

public class ResultPresenter : MonoBehaviour
{
    // ここはSerializeFieldでインスペクターから参照を設定
    [SerializeField] private ResultScoreView _resultScoreView; 
    [SerializeField] private ResultScoreModel _resultScoreModel = new ResultScoreModel(); 

    private async void Start()
    {
        // Viewの初期化を指示
        if (_resultScoreView != null)
        {
            _resultScoreView.Initialize();
        }


        // Modelの初期化（ダミースコア生成）を指示
        if (_resultScoreModel != null)
        {
            _resultScoreModel.ScoreGen();
        }

        
        await DisplaySingleScore(); // 1つのスコアを表示
    }

    /// <summary>
    /// 1つのスコアを非同期で表示する
    /// </summary>
    public async UniTask DisplaySingleScore()
    {
        Debug.Log("Loading single score...");
        
        // Modelから1つのスコアを非同期で取得
        int score = await _resultScoreModel.GetScore();


        // Viewにスコアの表示アニメーションを指示
        await _resultScoreView.DisplaySingleScoreAnimation(score);
    }
}