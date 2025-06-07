using UnityEngine;


namespace InGame.View
{
    public class MotherShipView : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody2D Rb;
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
            /*
            if (Rb == null)
            {
                Rb = GetComponent<Rigidbody2D>();
            }
            */
            
            
        }

        #endregion
        
        # region アクセスメソッド

        public Rigidbody2D GetRb() { return Rb; }

        #endregion
    }
}

