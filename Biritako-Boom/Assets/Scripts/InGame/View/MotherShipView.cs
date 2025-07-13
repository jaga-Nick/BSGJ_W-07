using System;
using UnityEngine;
using UnityEngine.Serialization;


namespace InGame.View
{
    public class MotherShipView : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rb;


        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }
        
        # region アクセスメソッド

        public Rigidbody2D GetRb() { return rb; }

        #endregion
    }
}

