using UnityEngine.SceneManagement;

namespace FrostfallSaga.Utils.Scenes
{
    public class SceneTransitioner : MonoBehaviourPersistingSingleton<SceneTransitioner>
    {
        /// <summary>
        ///     Will later interact with the UI to start a fade in animation before loading the scene.
        /// </summary>
        /// <param name="sceneName">The name of the scene to load (filename).</param>
        public static void FadeInToScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}