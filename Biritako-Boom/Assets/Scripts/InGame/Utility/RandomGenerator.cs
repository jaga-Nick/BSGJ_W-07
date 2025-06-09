using Random = UnityEngine.Random;

namespace InGame.Utility
{
    public class RandomGenerator
    {
        public float DetermineRandomCoordinates(float min, float max)
        {
            float value;
            do { value = Random.Range(min, max); } while (value is >= 0.0f and <= 1.0f);
            return value;
        }
    }
}