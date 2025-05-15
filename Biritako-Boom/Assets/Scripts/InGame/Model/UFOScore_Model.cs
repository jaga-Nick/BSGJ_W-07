using System;
using UnityEngine;

namespace UFO
{
    public class UFOScore_Model : MonoBehaviour
    {
        private int _score = 100;
        private int _multiplier = 1;



        public void ResetMulti()
        {
            _multiplier = 1;
        }

        public int AddScore()
        {
            _score *= _multiplier;
            //IncreaseMulti();  // ”{—¦‚ð‚©‚¯‚½‚¢‚È‚çŽg‚Á‚Ä‚­‚¾‚³‚¢
            return _score;
        }

        public void IncreaseMulti()
        {
            _multiplier *= 2;
        }
    }

}

