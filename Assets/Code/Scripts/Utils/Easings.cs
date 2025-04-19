using UnityEngine;

namespace FrostfallSaga.Utils
{
    public static class Easings
    {
        // Easing functions for animations
        // These functions can be used to create smooth transitions in animations
        // The parameter t should be in the range [0, 1]
        // The return value will also be in the range [0, 1]
        
        public static float Linear(float t)
        {
            return t; // No easing, just a linear transition
        }

        public static float EaseInQuad(float t)
        {
            return t * t; // Accelerates from zero
        }

        public static float EaseOutQuad(float t)
        {
            return t * (2f - t); // Decelerates toward the end
        }

        public static float EaseInOutQuad(float t)
        {
            if (t < 0.5f)
            {
                return 2f * t * t; // Accelerates from zero
            }
            else
            {
                return -1f + (4f * t) - (2f * t * t); // Decelerates toward the end
            }
        }

        public static float EaseInCubic(float t)
        {
            return t * t * t; // Accelerates from zero
        }

        public static float EaseOutCubic(float t)
        {
            return 1f - Mathf.Pow(1f - t, 3); // Decelerates toward the end
        }

        public static float EaseInOutCubic(float t)
        {
            if (t < 0.5f)
            {
                return 4f * t * t * t; // Accelerates from zero
            }
            else
            {
                return 1f - Mathf.Pow(-2f * t + 2f, 3) / 2f; // Decelerates toward the end
            }
        }
    }
}