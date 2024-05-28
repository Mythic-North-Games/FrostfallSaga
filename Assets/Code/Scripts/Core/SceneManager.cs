using UnityEngine;
using UnityEngine.SceneManagement;

namespace FrostfallSaga.Core
{
    public class SceneChanger : MonoBehaviour
    {
        public void ChangeSceneByIndex(int sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex);
        }
    }
}
