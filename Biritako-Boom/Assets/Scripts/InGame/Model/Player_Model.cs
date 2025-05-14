using UnityEngine;

namespace InGame
{
    /// <summary>
    /// Player
    /// </summary>
    public class Player_Model
    {
        private Vector3 InstancePosition=new Vector3(0,0,0);
        public float Speed { get; private set; } = 3.0f;


        public float GetSpeed()
        {
            return Speed;
        }

        public Vector3 GetInstancePosition()
        {
            return InstancePosition;
        }
    }
}