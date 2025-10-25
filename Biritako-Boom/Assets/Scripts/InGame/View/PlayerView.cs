using InGame.Model;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


namespace InGame.View
{
    /// <summary>
    /// PlayerAnimation変更処理
    /// </summary>

    [Serializable]
    public class PlayerView
    {
        

        private PlayerModel _model;

        //延長コード
        [FormerlySerializedAs("CodeGauge")] [SerializeField]
        private Image codeGauge;
         //Animation(実装までかなり長いはずなので全文コメント
        private Animator _animator;
        private Rigidbody2D _rb;

        //アニメーターのパラメーターをハッシュ化
        private static readonly int Move = Animator.StringToHash("Move");
        private static readonly int HaveConcent = Animator.StringToHash("HaveConcent");


        public void Init()
        {
            _animator = _model.PlayerObject.GetComponent<Animator>();
            _rb = _model.Rb;
        }

        // Update is called once per frame
        public void AnimationUpdate()
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

            if(move.x  > 0)
            {
                _model.PlayerObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else if (move.x <0)
            {
                _model.PlayerObject.transform.rotation = Quaternion.Euler(0, 180, 0);

            }
        }

        public void SetPlayerModel(PlayerModel model)
        {
            this._model = model;
        }

        public void SetHaveConcent(bool value)
        {
            _animator.SetBool(HaveConcent, value);
        }

        public void DisplayCodeGauge(float gaugePercent)
        {
            codeGauge.fillAmount = gaugePercent;
        }
    }
}