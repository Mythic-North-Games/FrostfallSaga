using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Utils.UI
{
    public static class CommonUIAnimations
    {
        public static IEnumerator PlayShakeAnimation(VisualElement element)
        {
            // Simulate a shake animation with randomized offsets and easing
            Vector3 originalPosition = element.transform.position;
            System.Random random = new();
            for (int i = 0; i < 5; i++)
            {
                float horizontalOffset = (float)(random.NextDouble() * 10f - 5f); // Random between -5 and 5
                float verticalOffset = (float)(random.NextDouble() * 10f - 5f);   // Random between -5 and 5
                Vector3 shakeOffset = new(horizontalOffset, verticalOffset, 0f);

                // Apply easing by interpolating the position
                for (float t = 0; t < 1; t += Time.deltaTime / 0.05f)
                {
                    element.transform.position = Vector3.Lerp(originalPosition, originalPosition + shakeOffset, t);
                    yield return null;
                }

                for (float t = 0; t < 1; t += Time.deltaTime / 0.05f)
                {
                    element.transform.position = Vector3.Lerp(originalPosition + shakeOffset, originalPosition, t);
                    yield return null;
                }
            }
            element.transform.position = originalPosition; // Reset to original position
        }

        public static IEnumerator PlayScaleAnimation(VisualElement element, Vector2 scalingFactor, float duration = 0.5f)
        {
            Vector3 originalScale = element.transform.scale;
            Vector3 targetScale = new(originalScale.x * scalingFactor.x, originalScale.y * scalingFactor.y, originalScale.z);

            // Smooth scaling up
            for (float t = 0; t < duration / 2; t += Time.deltaTime)
            {
                float normalizedTime = Mathf.SmoothStep(0, 1, t / (duration / 2));
                element.transform.scale = Vector3.Lerp(originalScale, targetScale, normalizedTime);
                yield return null;
            }

            // Ensure it ends at the target scale smoothly
            element.transform.scale = targetScale;

            // Smooth scaling down
            for (float t = 0; t < duration / 2; t += Time.deltaTime)
            {
                float normalizedTime = Mathf.SmoothStep(0, 1, t / (duration / 2));
                element.transform.scale = Vector3.Lerp(targetScale, originalScale, normalizedTime);
                yield return null;
            }

            // Ensure it resets to the original scale smoothly
            element.transform.scale = originalScale;
        }
    }
}