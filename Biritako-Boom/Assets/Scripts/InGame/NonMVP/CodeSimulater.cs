using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Net;
using System.Threading;
using System;
using InGame.Model;

namespace InGame.NonMVP
{
    /// <summary>
    /// Codeシミュレーター(GenerateCodeSystem-Classから生成する。)
    /// </summary>
    public class CodeSimulater : MonoBehaviour
    {
        /// <summary>
        /// 初期化データ
        /// </summary>
        /// <param name="lineRenderer"></param>
        /// <param name="particleCount"></param>
        /// <param name="timeStep"></param>
        /// <param name="gravity"></param>
        /// <param name="damping"></param>
        /// <param name="stiffness"></param>
        /// <param name="Start"></param>
        /// <param name="End"></param>
        public void Initialize(LineRenderer lineRenderer, int particleCount, float timeStep,
            Vector3 gravity, float damping, float stiffness,
            GameObject startObject,GameObject endObject,
            int explosionTriggerDistance,int maxExplosion
            )
        {
            this.CodeLineRenderer = lineRenderer;

            this.ParticleCount = particleCount;
            this.TimeStep = timeStep;
            this.Gravity = gravity;
            this.Damping = damping;
            this.Stiffness = stiffness;

            this.StartObject = startObject;
            this.EndObject = endObject;

            this.ExplosionTriggerDistance = explosionTriggerDistance;
            this.MaxExplosion = maxExplosion;

            InitializeRope();
            //位置設定の総量変更
            if (lineRenderer != null)
            {
                lineRenderer.positionCount = ParticleCount;
            }
        }
        /// <summary>
        /// 始点（家電）
        /// </summary>
        private GameObject StartObject;
        /// <summary>
        /// 終点（プレイヤーかその場）
        /// </summary>
        private GameObject EndObject;

        //爆発関係
        private int ExplosionTriggerDistance=3;
        private int MaxExplosion=4;

        #region データの実装（外部から設定される。Initializeで設定)
        // ヒモの粒子数
        private int ParticleCount = 20;

        // シミュレーションパラメータ
        private float TimeStep = 0.02f;
        private Vector3 Gravity = new Vector3(0, 0, 0);
        private float Damping = 2f;
        private float Stiffness = 2f;

        // 質点の状態
        private Vector3[] Positions;
        private Vector3[] Velocities;
        private float[] Masses;
        private bool[] IsFixed;

        // 拘束
        private struct Constraint
        {
            public int i, j;
            public float restLength;
            public Constraint(int i, int j, float restLength)
            {
                this.i = i;
                this.j = j;
                this.restLength = restLength;
            }
        }
        private List<Constraint> constraints = new List<Constraint>();
        private LineRenderer CodeLineRenderer;
        #endregion

        [Header("アニメーション設定")]
        [Tooltip("終点が始点まで移動するのにかかる時間（秒）")]
        public float shrinkDuration = 2.0f;

        // タスクのキャンセルに使用する
        private CancellationTokenSource cts;
        //ーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーー
        //ーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーー
        public void Update()
        {
            Simulate();
            UpdateLineRenderer();

        }


        /// <summary>
        /// ここで当たった時に消える。
        /// </summary>
        /// <param name="collision"></param>
        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject != StartObject && collision.gameObject !=EndObject)
            {
                Debug.Log("コードに何か衝突しました。デモ時はまだ何もさせていません。");
            }
        }

        /// <summary>
        /// Codeを置いた時のイベント。
        /// </summary>
        private void PutCodeEvent()
        {
            //拾う判定を作る為に。
            EndObject = new GameObject("EndPoint");
            EndObject.transform.SetParent(this.transform); // 親を this の Transform に設定
            EndObject.transform.position = EndObject.transform.position;
            CircleCollider2D circle=EndObject.AddComponent<CircleCollider2D>();
            EndObject.AddComponent<CodeEndPointAttach>();
            //コライダーの情報セット
            //コライダーの情報セット
            circle.radius = 80;
            circle.offset = new Vector2(0,0);

            //判定の為、Rigidbodyを作成。
            Rigidbody2D rb=EndObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;

            cts?.Cancel();
            cts = new CancellationTokenSource();


            //最終地点に進める。その後消える。
            MovePointAsync(cts.Token).Forget();
        }
        /// <summary>
        /// 拾うイベント（まだ最悪かかなくていいか。）
        /// </summary>
        private void GetCodeEvent()
        {

        }

        /// <summary>
        /// EndPointをStartPointの位置まで、指定した時間をかけて動かす非同期メソッド
        /// </summary>
        public async UniTask MovePointAsync(CancellationToken token)
        {
            // GameObjectが破棄された時に自動でキャンセルされるようにトークンをリンク
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, this.GetCancellationTokenOnDestroy());
            var linkedToken = linkedCts.Token;

            try
            {
                // アニメーション開始時の各位置を記録
                Vector3 initialEndPos = EndObject.transform.position;
                Vector3 targetPos = StartObject.transform.position;
                float elapsedTime = 0f;

                // 経過時間が指定したアニメーション時間に達するまでループ
                while (elapsedTime < shrinkDuration)
                {
                    // キャンセル要求があったら、例外を投げて処理を中断
                    linkedToken.ThrowIfCancellationRequested();

                    float t = elapsedTime / shrinkDuration;
                    EndObject.transform.position = Vector3.Lerp(initialEndPos, targetPos, t);

                    elapsedTime += Time.deltaTime;

                    // 1フレーム待機 (yield return null; のUniTask版)
                    await UniTask.Yield(PlayerLoopTiming.Update, linkedToken);
                }

                // ループ終了後、正確に目標位置に設定する
                EndObject.transform.position = targetPos;
            }
            catch (OperationCanceledException)
            {
                // キャンセルされた場合はここに飛ぶ（特に何もしなくても良い）
                Debug.Log("移動タスクがキャンセルされました。");
            }
            finally
            {
                // タスク完了またはキャンセルの後始末
                cts?.Dispose();
                cts = null;

                //このオブジェクトを最終的に削除。
                Destroy(gameObject);
            }
        }


        /// <summary>
        /// 爆破を呼び出す
        /// </summary>
        public void Explosion()
        {
            //ここでどれだけ離れているかを設定する
            GenerateExplosionManager generater = GenerateExplosionManager.Instance();
            //紐の総長さを計算し、基準に地点を
            float totalDistance = TotalDistance();
            int num=(int)totalDistance / ExplosionTriggerDistance;
            //最大４以上の爆発
            if (num >MaxExplosion)
            {
                num = MaxExplosion;
            }
            else if (num < 1)
            {
                num = 1;
            }
            int i = 0;
            int count = 0;
            while (count < num)
            {
                // 発火位置を均等に取り出す
                i = (int)((float)(Positions.Length - 1) * count / (num - 1));

                // 最後に1つだけだと 0除算になるのを防ぐ
                if (num == 1) i = (Positions.Length - 1) / 2;

                //爆発させる。（線の途中）
                generater.Factory(Positions[i], 0);
                count++;
            }
        }

        /// <summary>
        /// 総距離を求める（これによって延長コードゲージを増減させる。）
        /// </summary>
        /// <returns></returns>
        public float TotalDistance()
        {
            float totalDistance = 0f;
            for (int j = 0; j < Positions.Length - 1; j++)
            {
                totalDistance += Vector3.Distance(Positions[j], Positions[j + 1]);
            }
            return totalDistance;
        }




        #region ロープの設定
        /// <summary>
        /// ロープの質点と拘束を初期化する。
        /// 両端は固定され、中間は補間で配置される。
        /// </summary>
        private void InitializeRope()
        {
            Positions = new Vector3[ParticleCount];
            Velocities = new Vector3[ParticleCount];
            Masses = new float[ParticleCount];
            IsFixed = new bool[ParticleCount];

            for (int i = 0; i < ParticleCount; i++)
            {
                float t = (float)i / (ParticleCount - 1);
                Positions[i] = Vector3.Lerp(StartObject.transform.position, EndObject.transform.position, t);
                Velocities[i] = Vector3.zero;
                Masses[i] = 1f;
            }

            constraints.Clear();
            for (int i = 0; i < ParticleCount - 1; i++)
            {
                float restLength = Vector3.Distance(Positions[i], Positions[i + 1]);
                constraints.Add(new Constraint(i, i + 1, restLength));
            }

            IsFixed[0] = true;
            IsFixed[ParticleCount - 1] = true;
        }
        /// <summary>
        /// シミュレーション本体。力適用 → 拘束解決 → 状態更新 の流れ。
        /// </summary>
        private void Simulate()
        {
            // 重力など外力の適用
            for (int i = 0; i < ParticleCount; i++)
            {
                if (IsFixed[i]) continue;
                Velocities[i] += Gravity * TimeStep;
            }

            // 減衰
            for (int i = 0; i < ParticleCount; i++)
            {
                Velocities[i] *= (1 - Damping);
            }

            // 位置予測
            Vector3[] predicted = new Vector3[ParticleCount];
            for (int i = 0; i < ParticleCount; i++)
            {
                predicted[i] = Positions[i] + Velocities[i] * TimeStep;
            }

            // 拘束解決（反復）
            for (int iter = 0; iter < 5; iter++)
            {
                foreach (var c in constraints)
                {
                    Vector3 delta = predicted[c.i] - predicted[c.j];
                    float d = delta.magnitude;
                    float diff = (d - c.restLength) / d;

                    Vector3 correction = delta * diff * 0.5f * Stiffness;

                    if (!IsFixed[c.i]) predicted[c.i] -= correction;
                    if (!IsFixed[c.j]) predicted[c.j] += correction;
                }
            }

            // 状態の確定（速度と位置）
            for (int i = 0; i < ParticleCount; i++)
            {
                if (IsFixed[i]) continue;
                Velocities[i] = (predicted[i] - Positions[i]) / TimeStep;
                Positions[i] = predicted[i];
            }

            // 両端を強制的に固定
            Positions[0] = StartObject.transform.position;
            Positions[ParticleCount - 1] = EndObject.transform.position;
        }
        /// <summary>
        /// LineRendererをロープの現在の状態に合わせて更新。
        /// </summary>
        private void UpdateLineRenderer()
        {
            if (CodeLineRenderer == null) return;
            for (int i = 0; i < ParticleCount; i++)
            {
                CodeLineRenderer.SetPosition(i, Positions[i]);
            }
        }
        #endregion

        // このコンポーネントが無効になったり破棄された時に、実行中のタスクを止める
        private void OnDisable()
        {
            cts?.Cancel();
        }
    }
}