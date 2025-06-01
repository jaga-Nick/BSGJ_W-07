using UnityEngine;
using System.Collections.Generic;

public class PBDRopeManage : MonoBehaviour
{
    // ヒモの両端を指定するためのTransform
    [Header("両端の位置")]
    public Transform startPoint;
    public Transform endPoint;

    // ヒモの粒子数
    [Header("粒子数")]
    public int particleCount = 20;

    // シミュレーションパラメータ
    [Header("計算制度(更新間秒)")]
    public float timeStep = 0.02f;
    [Header("重力(質点にかかるもの)")]
    public Vector3 gravity = new Vector3(0, 0, 0);
    [Header("速度の減衰係数。\r\nシミュレーションの振動やエネルギーを抑えるために速度にかける減速率。")]
    public float damping = 2f;
    [Header("\t拘束の硬さを表す係数。\r\n値が大きいほどヒモの距離拘束が強く、伸び縮みしにくくなる。")]
    public float stiffness = 2f;

    // 質点の状態
    private Vector3[] positions;
    private Vector3[] velocities;
    private float[] masses;
    private bool[] isFixed;

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


    [Header("ロープの可視化用 LineRenderer")]
    public LineRenderer lineRenderer;

    //ーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーー
    //ーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーー


    private void Start()
    {
        InitializeRope();

        //位置設定の総量変更
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = particleCount;
        }
    }

    void Update()
    {
        Simulate();

        UpdateLineRenderer();
    }

    /// <summary>
    /// ロープの質点と拘束を初期化する。
    /// 両端は固定され、中間は補間で配置される。
    /// </summary>
    private void InitializeRope()
    {
        positions = new Vector3[particleCount];
        velocities = new Vector3[particleCount];
        masses = new float[particleCount];
        isFixed = new bool[particleCount];

        for (int i = 0; i < particleCount; i++)
        {
            float t = (float)i / (particleCount - 1);
            positions[i] = Vector3.Lerp(startPoint.position, endPoint.position, t);
            velocities[i] = Vector3.zero;
            masses[i] = 1f;
        }

        constraints.Clear();
        for (int i = 0; i < particleCount - 1; i++)
        {
            float restLength = Vector3.Distance(positions[i], positions[i + 1]);
            constraints.Add(new Constraint(i, i + 1, restLength));
        }

        isFixed[0] = true;
        isFixed[particleCount - 1] = true;
    }

    /// <summary>
    /// シミュレーション本体。力適用 → 拘束解決 → 状態更新 の流れ。
    /// </summary>
    private void Simulate()
    {
        // 重力など外力の適用
        for (int i = 0; i < particleCount; i++)
        {
            if (isFixed[i]) continue;
            velocities[i] += gravity * timeStep;
        }

        // 減衰
        for (int i = 0; i < particleCount; i++)
        {
            velocities[i] *= (1 - damping);
        }

        // 位置予測
        Vector3[] predicted = new Vector3[particleCount];
        for (int i = 0; i < particleCount; i++)
        {
            predicted[i] = positions[i] + velocities[i] * timeStep;
        }

        // 拘束解決（反復）
        for (int iter = 0; iter < 5; iter++)
        {
            foreach (var c in constraints)
            {
                Vector3 delta = predicted[c.i] - predicted[c.j];
                float d = delta.magnitude;
                float diff = (d - c.restLength) / d;

                Vector3 correction = delta * diff * 0.5f * stiffness;

                if (!isFixed[c.i]) predicted[c.i] -= correction;
                if (!isFixed[c.j]) predicted[c.j] += correction;
            }
        }

        // 状態の確定（速度と位置）
        for (int i = 0; i < particleCount; i++)
        {
            if (isFixed[i]) continue;
            velocities[i] = (predicted[i] - positions[i]) / timeStep;
            positions[i] = predicted[i];
        }

        // 両端を強制的に固定
        positions[0] = startPoint.position;
        positions[particleCount - 1] = endPoint.position;
    }

    /// <summary>
    /// LineRendererをロープの現在の状態に合わせて更新。
    /// </summary>
    private void UpdateLineRenderer()
    {
        if (lineRenderer == null) return;
        for (int i = 0; i < particleCount; i++)
        {
            lineRenderer.SetPosition(i, positions[i]);
        }
    }
}
