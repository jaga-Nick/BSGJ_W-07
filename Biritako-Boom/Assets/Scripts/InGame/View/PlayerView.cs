using InGame.Model;
using System;
using UnityEngine;
using UnityEngine.UI;


namespace InGame.View
{
    /// <summary>
    /// PlayerAnimation変更処理
    /// </summary>

    [Serializable]
    public class PlayerView
    {
        

        private PlayerModel model;

        //延長コード
        [SerializeField]
        private Image codeGauge;
         //Animation(実装までかなり長いはずなので全文コメント
        private Animator animator;
        private Rigidbody2D rb;

        //アニメーターのパラメーターをハッシュ化
        private static readonly int move = Animator.StringToHash("Move");
        private static readonly int haveConcent = Animator.StringToHash("HaveConcent");


        public void Init()
        {
            animator = model.playerObject.GetComponent<Animator>();
            rb = model.rb;
        }

        // Update is called once per frame
        public void AnimationUpdate()
        {
            // velocityを取得して移動しているかどうか
            var move = rb.linearVelocity;

            //動いたら
            if(move.x == 0 && move.y==0)
            {
                animator.SetBool(PlayerView.move,false);
            }
            else
            {
                animator.SetBool(PlayerView.move, true);
            }

            if(move.x  > 0)
            {
                model.playerObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else if (move.x <0)
            {
                model.playerObject.transform.rotation = Quaternion.Euler(0, 180, 0);

            }
        }

        public void SetPlayerModel(PlayerModel _model)
        {
            model = _model;
        }

        public void SetHaveConcent(bool value)
        {
            animator.SetBool(haveConcent, value);
        }

        public void DisplayCodeGauge(float GaugePercent)
        {
            codeGauge.fillAmount = GaugePercent;
        }
    }
}