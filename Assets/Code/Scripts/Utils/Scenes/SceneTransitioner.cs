using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace FrostfallSaga.Utils.Scenes
{
    public class SceneTransitioner : MonoBehaviourPersistingSingleton<SceneTransitioner>
    {
        // Loading scene settings
        private string _loadingSceneName;
        private static readonly float MIN_LOADING_TIME = 2f;

        // Fade settings
        private UIDocument _uiDocument;
        private float _fadeDuration;
        private static readonly string FADE_OVERLAY_UI_NAME = "FadeOverlay";

        private VisualElement _fadeOverlay;

        protected override void Init()
        {
            _uiDocument ??= GetComponent<UIDocument>();
            if (_uiDocument == null)
            {
                Debug.LogError("SceneTransitioner: UIDocument is not assigned.");
                return;
            }

            _fadeOverlay = _uiDocument.rootVisualElement.Q<VisualElement>(FADE_OVERLAY_UI_NAME);
            if (_fadeOverlay == null)
            {
                Debug.LogError($"SceneTransitioner: Fade overlay element '{FADE_OVERLAY_UI_NAME}' not found in UIDocument.");
                return;
            }

            _fadeOverlay.style.opacity = 0f;
            _fadeOverlay.pickingMode = PickingMode.Ignore;
        }

        public static void BrutalSceneTransition(string targetScene, float fadeDuration = 0.5f)
        {
            Instance._fadeOverlay.style.opacity = 1f;
            Instance.StartCoroutine(Instance.Fade(0f, fadeDuration));
            SceneManager.LoadScene(targetScene);
        }

        public static void FadeInCurrentScene(float fadeDuration = 0.5f)
        {
            Instance._fadeOverlay.style.opacity = 1f;
            Instance.StartCoroutine(Instance.Fade(0f, fadeDuration));
        }

        public static void TransitionToScene(string targetScene, string loadingSceneName = "LoadingScene", float fadeDuration = 0.5f)
        {
            Instance._loadingSceneName = loadingSceneName;
            Instance._fadeDuration = fadeDuration;
            Instance.StartCoroutine(Instance.SceneTransitionCoroutine(targetScene));
        }

        private IEnumerator SceneTransitionCoroutine(string targetScene)
        {
            // Fade Out current screen
            yield return StartCoroutine(Fade(1f, _fadeDuration));

            // Delete audio and event listener of the current scene
            DeleteAllOfTypeFromScene<AudioSource>();
            DeleteAllOfTypeFromScene<AudioListener>();
            Destroy(GameObject.Find("EventSystem"));

            // Load loading scene additively
            AsyncOperation loadingSceneOp = SceneManager.LoadSceneAsync(_loadingSceneName, LoadSceneMode.Additive);
            while (!loadingSceneOp.isDone)
                yield return null;

            // Fade in loading screen
            yield return StartCoroutine(Fade(0f, _fadeDuration));

            // Begin loading target scene and wait minimum time
            float startTime = Time.time;
            AsyncOperation targetSceneOp = SceneManager.LoadSceneAsync(targetScene);
            targetSceneOp.allowSceneActivation = false;

            while (Time.time - startTime < MIN_LOADING_TIME || targetSceneOp.progress < 0.9f)
                yield return null;

            // Fade to black
            yield return StartCoroutine(Fade(1f, _fadeDuration));

            // Activate target scene
            targetSceneOp.allowSceneActivation = true;
            while (!targetSceneOp.isDone)
                yield return null;

            // Unload loading scene
            yield return SceneManager.UnloadSceneAsync(_loadingSceneName);
        }

        private IEnumerator Fade(float targetAlpha, float duration)
        {
            if (_fadeOverlay == null) yield break;

            float startAlpha = _fadeOverlay.resolvedStyle.opacity;
            float time = 0f;

            _fadeOverlay.pickingMode = targetAlpha > 0f ? PickingMode.Position : PickingMode.Ignore;

            while (time < duration)
            {
                time += Time.deltaTime;
                float t = time / duration;
                float alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
                _fadeOverlay.style.opacity = alpha;
                yield return null;
            }

            _fadeOverlay.style.opacity = targetAlpha;
        }

        private void DeleteAllOfTypeFromScene<T>() where T : Component
        {
            FindObjectsOfType<T>().ToList().ForEach(audioSource => Destroy(audioSource));
        }
    }
}
