using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace InGame.View
{
    /// <summary>
    /// PlayerAnimation変更処理
    /// </summary>
    public class PlayerView : MonoBehaviour
    {
        //延長コード
        [SerializeField]
        private Image CodeGauge;
        /* Animation
        private Animator _animator;
        private Rigidbody2D _rb;

        //アニメーターのパラメーターをハッシュ化
        private static readonly int Vertical = Animator.StringToHash("Vertical");
        private static readonly int Horizontal = Animator.StringToHash("Horizontal");
        private static readonly int HorizontalOnMove = Animator.StringToHash("Horizontal_OnMove");
        private static readonly int VerticalOnMove = Animator.StringToHash("Vertical_OnMove");

        
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _rb = GetComponent<Rigidbody2D>();
        }

        // Update is called once per frame
        private void Update()
        {
            // velocityを取得して移動しているかどうか
            var move = _rb.linearVelocity;

            // 縦横の動きを送る。
            _animator.SetFloat(Vertical, move.y);
            _animator.SetFloat(Horizontal, move.x);

            // 縦横方向に0でなければ。
            _animator.SetBool(VerticalOnMove, move.y != 0);
            _animator.SetBool(HorizontalOnMove, move.x != 0);
        }*/
        public void DisplayCodeGauge(float GaugePercent)
        {
            CodeGauge.fillAmount = GaugePercent;
        }
    }
}