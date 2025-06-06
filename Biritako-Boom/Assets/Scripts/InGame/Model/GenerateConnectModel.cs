using System.Collections.Generic;
using UnityEngine;

namespace InGame
{
    /// <summary>
    /// Listで管理する。
    /// </summary>
    class StructConnect
    {
        
    }

    /// <summary>
    /// 
    /// </summary>
    public class GenerateConnectModel : MonoBehaviour
    {
        private Transform Init;
        private Transform End;
        #region　LineRendererの設定まとめ
        [Header("LineRendererの設定")]
        [SerializeField]
        //線の細さ太さ。
        private float LineWidth = 0.1f;
        [SerializeField]
        //マテリアルを設定(virtual)
        private Material LineMaterial;
        #endregion
        [Header("紐計算の設定")]
        // ヒモの粒子数
        [Header("粒子数")]
        [SerializeField]
        private int ParticleCount = 20;
        // シミュレーションパラメータ
        [Header("計算制度(更新間秒)")]
        [SerializeField]
        private float TimeStep = 0.02f;
        [Header("重力(質点にかかるもの)")]
        [SerializeField]
        private Vector3 Gravity = new Vector3(0, 0, 0);
        [Header("速度の減衰係数。\r\nシミュレーションの振動やエネルギーを抑えるために速度にかける減速率。")]
        [SerializeField]
        private float Damping = 2f;
        [Header("\t拘束の硬さを表す係数。\r\n値が大きいほどヒモの距離拘束が強く、伸び縮みしにくくなる。")]
        [SerializeField]
        private float Stiffness = 2f;

        /// <summary>
        /// コードを生成
        /// </summary>
        public void GenerateCode(GameObject Start, GameObject End)
        {
            GameObject CodeObject = new GameObject("Code");

            LineRenderer lineRenderer = CodeObject.AddComponent<LineRenderer>();
            //各種コードで設定
            lineRenderer.startWidth = LineWidth;
            lineRenderer.endWidth = LineWidth;
            lineRenderer.material = LineMaterial;

            //物理演算を線で行う為に生成。
            EdgeCollider2D edge = CodeObject.AddComponent<EdgeCollider2D>();
            edge.isTrigger = true;//衝突をオフに（IsTriggerイベントのみを取得する。）

            //ここでコードを生成する
            CodeSimulater codeSimulater = CodeObject.AddComponent<CodeSimulater>();
            //この時点ではUpdateは発生しない為問題ない
            //設定
            codeSimulater.Initialize(lineRenderer, ParticleCount, TimeStep, Gravity, Damping, Stiffness, Start.transform, End.transform);

            //オブジェクト設定

        }
    }
}