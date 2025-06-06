using UnityEngine;
using System.Collections.Generic;

public class CodeSimulater : MonoBehaviour
{
    public void Initialize(int particleCount,float timeStep,Vector3 gravity,float damping,float stiffness)
    {
        this.ParticleCount = particleCount;
        this.TimeStep = timeStep;
        this.Gravity = gravity;
        this.Damping = damping;
        this.Stiffness = stiffness;



        InitializeRope();
        //位置設定の総量変更
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = ParticleCount;
        }
    }

    // ヒモの両端を指定するためのTransform
    [Header("両端の位置")]
    public Transform startPoint;
    public Transform endPoint;

    // ヒモの粒子数
    [Header("粒子数")]
    [SerializeField] 
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


    [Header("ロープの可視化用 LineRenderer")]
    public LineRenderer lineRenderer;

    //ーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーー
    //ーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーー


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
        Positions = new Vector3[ParticleCount];
        Velocities = new Vector3[ParticleCount];
        Masses = new float[ParticleCount];
        IsFixed = new bool[ParticleCount];

        for (int i = 0; i < ParticleCount; i++)
        {
            float t = (float)i / (ParticleCount - 1);
            Positions[i] = Vector3.Lerp(startPoint.position, endPoint.position, t);
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
        Positions[0] = startPoint.position;
        Positions[ParticleCount - 1] = endPoint.position;
    }

    /// <summary>
    /// LineRendererをロープの現在の状態に合わせて更新。
    /// </summary>
    private void UpdateLineRenderer()
    {
        if (lineRenderer == null) return;
        for (int i = 0; i < ParticleCount; i++)
        {
            lineRenderer.SetPosition(i, Positions[i]);
        }
    }
}
