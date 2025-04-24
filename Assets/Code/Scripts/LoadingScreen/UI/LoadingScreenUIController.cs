using System.Collections;
using FrostfallSaga.Core.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.LoadingScreen.UI
{
    public class LoadingScreenUIController : BaseUIController
    {
        #region UXML Names & classes
        private static readonly string LOADING_LABEL_UI_NAME = "LoadingLabel";
        #endregion

        private Label _loadingLabel;

        private void Awake()
        {
            _loadingLabel = _uiDoc.rootVisualElement.Q<Label>(LOADING_LABEL_UI_NAME);
        }

        private void Start()
        {
            StartCoroutine(SimulateLoadingWithDots());
        }

        private IEnumerator SimulateLoadingWithDots()
        {
            while (true)
            {
                _loadingLabel.text = "Loading";
                yield return new WaitForSeconds(0.25f);
                _loadingLabel.text = "Loading.";
                yield return new WaitForSeconds(0.25f);
                _loadingLabel.text = "Loading..";
                yield return new WaitForSeconds(0.25f);
                _loadingLabel.text = "Loading...";
                yield return new WaitForSeconds(0.25f);
            }
        }
    }
}