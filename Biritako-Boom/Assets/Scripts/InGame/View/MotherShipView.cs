using System;
using UnityEngine;


namespace InGame.View
{
    public class MotherShipView : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody2D _rb;


        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        void Start()
        {
            
        }
        
        void Update()
        {
            
        }
        
        #region コンストラクタ

        /// <summary>
        /// MotherShipViewのコンストラクタ
        /// </summary>
        public MotherShipView()
        {

            
            
        }

        #endregion
        
        # region アクセスメソッド

        public Rigidbody2D GetRb() { return _rb; }

        #endregion
    }
}

