using InGame.Model;
using System;
using TMPro;
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
        private Image CodeGauge;
         //Animation(実装までかなり長いはずなので全文コメント
        private Animator _animator;
        private Rigidbody2D _rb;

        //アニメーターのパラメーターをハッシュ化
        private static readonly int Move = Animator.StringToHash("Move");
        private static readonly int HaveConcent = Animator.StringToHash("HaveConcent");


        public void Init()
        {
            _animator = model.PlayerObject.GetComponent<Animator>();
            _rb = model.Rb;
        }

        // Update is called once per frame
        public void AnimmationUpdate()
        {
            // velocityを取得して移動しているかどうか
            var move = _rb.linearVelocity;

            //動いたら
            if(move.x == 0 && move.y==0)
            {
                _animator.SetBool(Move,false);
            }
            else
            {
                _animator.SetBool(Move, true);
            }
        }

        public void SetPlayerModel(PlayerModel _model)
        {
            model = _model;
        }

        public void SetHaveConcent(bool value)
        {
            _animator.SetBool(HaveConcent, value);
        }

        public void DisplayCodeGauge(float GaugePercent)
        {
            CodeGauge.fillAmount = GaugePercent;
        }
    }
}