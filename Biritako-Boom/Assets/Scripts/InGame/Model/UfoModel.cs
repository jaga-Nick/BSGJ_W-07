using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

namespace InGame.Model
{
    /// <summary>
    /// UFO(敵)の管理クラス
    /// </summary>
    public class UfoModel : MonoBehaviour
    {
        /// <summary>
        /// UFOのHPに応じてのイベント管理
        /// </summary>
        public event Action UfoHpChanged;
        
        /// <summary>
        /// UFOのHP管理のフィールド
        /// </summary>
        private const int minUfoHp = 0;
        private const int maxUfoHp = 100;
        private int currentUfoHp;

        private const int score = 100;
        private int currentScore;
        
        /// <summary>
        /// ゲッターとセッター。読み取り専用のプロパティ。
        /// </summary>
        public int CurrentUfoHp { get => currentUfoHp; set => currentUfoHp = value; }
        public int CurrentScore { get => currentScore; set => currentScore = value; }
        public int MinUfoHp => minUfoHp;
        public int MaxUfoHp => maxUfoHp;

        /// <summary>
        /// UFOのHPをamountに応じて増やす。
        /// </summary>
        /// <param name="amount"></param>
        public void IncrementUfoHp(int amount)
        {
            currentUfoHp += amount;
            currentUfoHp = Mathf.Clamp(currentUfoHp, minUfoHp, maxUfoHp);
            UpdateUfoHp();
        }

        /// <summary>
        /// UFOのHPをamountに応じて減らす。
        /// </summary>
        /// <param name="amount"></param>
        public void DecrementUfoHp(int amount)
        {
            currentUfoHp -= amount;
            CurrentUfoHp = Mathf.Clamp(currentUfoHp, minUfoHp, maxUfoHp);
            UpdateUfoHp();
        }

        /// <summary>
        /// UFOが死んだかどうかのブール判定。
        /// </summary>
        /// <returns></returns>
        public bool IsDead()
        {
            return currentUfoHp <= 0;
        }

        /// <summary>
        /// UFOのHPをもとに戻す。全回復。
        /// </summary>
        public void RestoreUfoHp()
        {
            currentUfoHp = maxUfoHp;
        }

        /// <summary>
        /// HP変更イベント。
        /// </summary>
        private void UpdateUfoHp()
        {
            UfoHpChanged?.Invoke();
        }
        
        /// <summary>
        /// UFOを破壊する、
        /// </summary>
        /// <param name="ufoInstance"></param>
        public void DestroyUfo(GameObject ufoInstance)
        {
            if (!ufoInstance) return;
            Addressables.ReleaseInstance(ufoInstance); 
            ufoInstance = null;
        }
    }
}
