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
        public void Initialize(LineRenderer lineRenderer, int particleCount, float timeStep,
            Vector3 gravity, float damping, float stiffness,
            GameObject startObject, GameObject endObject,
            int explosionTriggerDistance, int maxExplosion
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

            InitializeEdgeCollider();


            // 1. プロジェクトに登録されている"Wall"レイヤーの番号を取得します。
            int wallLayerNumber = LayerMask.NameToLayer("Wall");

            // 2. レイヤーが正しく登録されているか確認します。
            if (wallLayerNumber == -1)
            {
                // もし"Wall"レイヤーが存在しない場合、エラーログを出して処理を中断します。
                Debug.LogError("「Wall」という名前のレイヤーがプロジェクトに登録されていません！ Edit > Project Settings > Tags and Layers から設定してください。");
            }
            else
            {
                // 3. 取得したレイヤー番号を使って、そのレイヤーだけを有効にするマスクを作成し、設定します。
                this.collisionMask = 1 << wallLayerNumber;
            }
        }
        /// <summary>
        /// 始点（家電）
        /// </summary>
        public GameObject StartObject { get; private set; }
        /// <summary>
        /// 終点（プレイヤーかその場、ソケット）
        /// </summary>
        private GameObject EndObject;

        //爆発関係
        private int ExplosionTriggerDistance = 3;
        private int MaxExplosion;

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

        [Header("衝突設定")]
        [Tooltip("衝突対象のレイヤー")]
        [SerializeField] private LayerMask collisionMask ;
        [Tooltip("各パーティクルの衝突判定半径")]
        [SerializeField] private float particleRadius = 0.1f;

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
            UpdateEdgeCollider();
        }


        /// <summary>
        /// ここで当たった時に消える。
        /// </summary>
        /// <param name="collision"></param>
        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject != StartObject && collision.gameObject != EndObject)
            {
                //Debug.Log("コードに何か衝突しました。デモ時はまだ何もさせていません。");
            }
        }

        /// <summary>
        /// Codeを置いた時のイベント。
        /// </summary>
        public void PutCodeEvent()
        {
            //拾う判定を作る為に。
            GameObject endPoint = new GameObject("EndPoint");
            endPoint.transform.SetParent(this.transform); // 親を this の Transform に設定

            Debug.Log(EndObject.name);
            Debug.Log(EndObject.transform.position);
            endPoint.transform.position = EndObject.transform.position;

            EndObject = endPoint;

            CircleCollider2D circle = EndObject.AddComponent<CircleCollider2D>();
            EndObject.AddComponent<CodeEndPointAttach>();
            //コライダーの情報セット
            circle.radius = 80;
            circle.offset = new Vector2(0, 0);

            //判定の為、Rigidbodyを作成。
            Rigidbody2D rb = EndObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;

            cts?.Cancel();
            cts = new CancellationTokenSource();


            //最終地点に進める。その後消える。
            MovePointAsync(cts.Token).Forget();
        }
        /// <summary>
        /// 拾うイベント（まだ最悪かかなくていいか。）
        /// </summary>
        private void GetCodeEvent(GameObject game)
        {

        }

        /// <summary>
        /// ソケットにコードを刺す
        /// 消費電力確定（未記入）
        /// </summary>
        public void InjectionSocketCode(GameObject socket)
        {
            EndObject = socket;
        }
        /// <summary>
        /// 戻す
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
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
                    //指定した秒数方向へ動かす。
                    EndObject.transform.position = Vector3.Lerp(initialEndPos, targetPos, t);

                    elapsedTime += Time.deltaTime;

                    // 1フレーム待機
                    await UniTask.Yield(PlayerLoopTiming.Update, linkedToken);
                }

                // ループ終了後、正確に目標位置に設定する
                EndObject.transform.position = targetPos;
            }
            catch (OperationCanceledException)
            {
                Debug.Log("移動タスクがキャンセルされました。");
            }
            finally
            {
                // タスク完了またはキャンセルの後始末
                cts?.Dispose();
                cts = null;

                if (this != null)
                {
                    Destroy(gameObject);
                }
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
            int num = (int)totalDistance / ExplosionTriggerDistance;

            //-----------------------------------------

            //ここに家電の爆発を書く（まだ書かない）


            //---------------------------------

            //最大４以上の爆発
            if (num >= MaxExplosion)
            {
                num = MaxExplosion;
            }
            else if (num < 1)
            {
                num = 1;
            }

            int i = 0;
            int count = 0;


            // 最後に1つだと0除算になるのは防ぐ
            if (num == 1)
            {
                //等分で爆発させる。
                i = (Positions.Length - 1) / 2;
                generater.Factory(Positions[i], 0);
            }
            else
            {
                //爆発参照
                int rate = Positions.Length / (num + 1);
                while (count < num)
                {
                    i += rate;
                    Debug.Log(i);
                    //爆発させる。（線の途中）
                    generater.Factory(Positions[i], 0);
                    count++;
                }
            }

            //このアタッチしているオブジェクトを全てスクリプトごと削除する。
            Destroy(gameObject);
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


        #region EdgeCollider関連


        private EdgeCollider2D _edgeCollider;
        // このメンバー変数を再利用します。
        private Vector2[] _colliderPoints;

        /// <summary>
        /// EdgeCollider2Dの初期設定を行います
        /// </summary>
        private void InitializeEdgeCollider()
        {
            Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            _edgeCollider = GetComponent<EdgeCollider2D>();
            if (_edgeCollider == null)
            {
                _edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
            }

            // コライダーの頂点配列を一度だけ確保し、ガベージコレクションの負荷を避けます。
            if (_colliderPoints == null || _colliderPoints.Length != ParticleCount)
            {
                _colliderPoints = new Vector2[ParticleCount];
            }
        }

        /// <summary>
        /// EdgeCollider2Dの形状を現在のロープの位置に合わせて更新します
        /// </summary>
        private void UpdateEdgeCollider()
        {
            if (_edgeCollider == null) return;

            // メンバー変数として保持している頂点配列に、現在のパーティクルの位置を格納します。
            for (int i = 0; i < ParticleCount; i++)
            {
                // C#はVector3をVector2へ暗黙的に変換（z座標を破棄）します。
                _colliderPoints[i] = Positions[i];
            }

            // 更新した頂点配列をEdgeCollider2Dに一括でセットすることで効率化します。
            _edgeCollider.points = _colliderPoints;
        }

        #endregion

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
        /// パーティクルの衝突を解決し、障害物から押し出す（ClosestPointを使った最も確実なバージョン）
        /// </summary>
        /// <param name="predictedPositions">予測されたパーティクルの位置配列</param>
        private void ResolveCollisions(Vector3[] predictedPositions)
        {
            for (int i = 0; i < ParticleCount; i++)
            {
                if (IsFixed[i]) continue;

                // 1. パーティクルの円範囲にあるコライダー候補を取得
                Collider2D[] hitColliders = Physics2D.OverlapCircleAll(predictedPositions[i], particleRadius, collisionMask);

                foreach (var hitCollider in hitColliders)
                {
                    if (hitCollider.gameObject == StartObject || hitCollider.gameObject == EndObject)
                    {
                        continue;
                    }

                    // 2. コライダー表面の「最も近い点」を取得する (非常に基本的なAPI)
                    Vector2 closestPoint = hitCollider.ClosestPoint(predictedPositions[i]);

                    // 3. パーティクル中心から、その「最も近い点」へのベクトルと距離を計算
                    Vector2 vectorToClosestPoint = closestPoint - (Vector2)predictedPositions[i];
                    float distance = vectorToClosestPoint.magnitude;

                    // 4. パーティクルの半径を考慮して、めり込みの深さを計算
                    float penetrationDepth = particleRadius - distance;

                    // 5. めり込みが少しでも発生している場合のみ、押し出す処理を行う
                    if (penetrationDepth > 0)
                    {
                        // 6. 押し出す方向（最も近い点から中心へ向かう方向）に、めり込んだ深さだけ位置を補正する
                        //    ゼロ除算を避けるためのチェック
                        if (distance > 0.0001f)
                        {
                            Vector2 pushDirection = -vectorToClosestPoint.normalized;
                            predictedPositions[i] += (Vector3)(pushDirection * penetrationDepth);
                        }
                    }
                }
            }
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
            for (int iter = 0; iter < 5; iter++) // イテレーション回数は必要に応じて調整
            {
                // 1. 距離の拘束を解決
                foreach (var c in constraints)
                {
                    Vector3 delta = predicted[c.i] - predicted[c.j];
                    float d = delta.magnitude;
                    if (d == 0) continue; // ゼロ除算を避ける
                    float diff = (d - c.restLength) / d;

                    Vector3 correction = delta * diff * 0.5f * Stiffness;

                    if (!IsFixed[c.i]) predicted[c.i] -= correction;
                    if (!IsFixed[c.j]) predicted[c.j] += correction;
                }

                // 2. 衝突の拘束を解決
                ResolveCollisions(predicted);
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