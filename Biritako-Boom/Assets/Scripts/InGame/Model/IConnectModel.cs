using UnityEngine;

//構造体としてのクラス
public class ConnectModel:MonoBehaviour
{
    private Transform Init;
    private Transform End;

    private LineRenderer lineRenderer;

    //線の細さ太さ。
    private float LineWidth = 0.1f;
    //マテリアルを設定(virtual)
    private Material LineMaterial;
    public void GenerateLineRenderer()
    {
        lineRenderer=gameObject.AddComponent<LineRenderer>();
        //各種コードで設定
        lineRenderer.startWidth = LineWidth;
        lineRenderer.endWidth = LineWidth;
        lineRenderer.material = LineMaterial;
    }
}
