using UnityEngine;
using UnityEngine.UI;

namespace InGame.View
{
    /// <summary>
    /// 母艦のHPゲージの見た目の更新を担当するView。
    /// 通常モードとダメージゲージモードの切り替えに対応。
    /// </summary>
    public class MotherShipHpGaugeView : MonoBehaviour
    {
        [Header("HPゲージ")]
        [SerializeField] private Image hpImage;
        
        [Header("ダメージゲージ")]
        [Tooltip("ダメージを表現する、HPゲージの背景にあるImage")]
        [SerializeField] private Image damageImage;
        [Tooltip("trueにすると、HPゲージが即時反映され、ダメージゲージが滑らかに追従するモードになります")]
        [SerializeField] private bool damageGauge = false;
        
        [Header("HPゲージの色設定")]
        [ColorUsage(false, false), SerializeField] private Color startColor = Color.green;
        [ColorUsage(false, false), SerializeField] private Color middleColor = Color.yellow;
        [ColorUsage(false, false), SerializeField] private Color endColor = Color.red;
        
        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            if (damageGauge)
            {
                // ダメージゲージモードの場合
                if (damageImage != null)
                {
                    damageImage.gameObject.SetActive(true);
                    damageImage.color = Color.red; // ダメージゲージは赤色固定
                }
                if (hpImage != null)
                {
                    hpImage.color = startColor; // HPゲージは開始色で固定
                }
            }
            else
            {
                // 通常モードの場合
                if (damageImage != null)
                {
                    damageImage.gameObject.SetActive(false); // ダメージゲージは非表示
                }
            }
        }

        /// <summary>
        /// ゲージの表示を更新
        /// </summary>

        public void UpdateView(float hpFill, float lerpFill)
        {
            if (hpImage == null) return;

            if (damageGauge)
            {
                // --- ダメージゲージモードの処理 ---
                hpImage.fillAmount = hpFill; // HPゲージは実際のHPに即時反映
                if (damageImage != null)
                {
                    damageImage.fillAmount = lerpFill; // ダメージゲージが滑らかに減る
                }
            }
            else
            {
                // --- 通常モードの処理 ---
                hpImage.fillAmount = lerpFill; // HPゲージ自体が滑らかに減る

                // HPの割合に応じて、緑→黄→赤と色を変化させる
                Color newColor;
                if (lerpFill > 0.5f)
                {
                    float t = (lerpFill - 0.5f) * 2f; 
                    newColor = Color.Lerp(middleColor, startColor, t);
                }
                else
                {
                    float t = lerpFill * 2f;
                    newColor = Color.Lerp(endColor, middleColor, t);
                }
                hpImage.color = newColor;
            }
        }

        public bool GetDamageGauge()
        {
            return damageGauge;
        }
    }
}
