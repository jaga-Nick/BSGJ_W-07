using UnityEngine;

namespace InGame.NonMVP
{
    /// <summary>
    /// コードのシミュレートオブジェクトを生成するクラス
    /// </summary>
    public class GenerateCodeSystem : MonoBehaviour
    {
        [Header("LineRendererの設定")]
        [SerializeField]
        //線の細さ太さ。
        private float lineWidth = 0.1f;
        [SerializeField]
        //マテリアルを設定(virtual)
        private Material lineMaterial;
        [Header("描画順")]
        [SerializeField]
        private int orderInLayer = 3;


        [Header("紐計算の設定")]
        // ヒモの粒子数
        [Header("粒子数")]
        [SerializeField]
        private int particleCount = 100;
        // シミュレーションパラメータ
        [Header("計算制度(更新間秒)")]
        [SerializeField]
        private float timeStep = 0.02f;
        [Header("重力(質点にかかるもの)")]
        [SerializeField]
        private Vector3 gravity = new Vector3(0, 0, 0);
        [Header("速度の減衰係数。\r\nシミュレーションの振動やエネルギーを抑えるために速度にかける減速率。")]
        [SerializeField]
        private float damping = 0.8f;
        [Header("\t拘束の硬さを表す係数。\r\n値が大きいほどヒモの距離拘束が強く、伸び縮みしにくくなる。")]
        [SerializeField]
        private float stiffness = 0.9f;

        //------------------------------------
        [Header("爆発の基準距離")]
        [SerializeField]
        private int explosionTriggerDistance= 1;
        [Header("最大コード爆発数")]
        [SerializeField]
        private int maxExplosion = 5;

        /// <summary>
        /// コードを生成
        /// </summary>
        public CodeSimulater GenerateCode(GameObject Start, GameObject End)
        {
            GameObject CodeObject = new GameObject("Code");

            LineRenderer lineRenderer = CodeObject.AddComponent<LineRenderer>();
            //LineRendererを修正する。
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
            lineRenderer.material = lineMaterial;
            lineRenderer.sortingOrder = orderInLayer;

            //物理演算を線で行う為に生成。
            EdgeCollider2D edge = CodeObject.AddComponent<EdgeCollider2D>();
            edge.isTrigger = true;//衝突をオフに（IsTriggerイベントのみを取得する。）

            //ここでコードを生成する
            CodeSimulater codeSimulater = CodeObject.AddComponent<CodeSimulater>();

            //この時点ではUpdateは発生しない為問題ない
            //設定
            codeSimulater.Initialize(
                lineRenderer,
                particleCount, 
                timeStep, 
                gravity, 
                damping, 
                stiffness, 
                Start , 
                End, 
                explosionTriggerDistance,
                maxExplosion);

            

            //オブジェクト設定
            return codeSimulater;
        }
    }
}