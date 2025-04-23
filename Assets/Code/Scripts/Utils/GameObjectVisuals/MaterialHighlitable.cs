using System;
using UnityEngine;

namespace FrostfallSaga.Utils.GameObjectVisuals
{
    /// <summary>
    ///     Add the possibility to highlight the visual aspects of the element through materials.
    /// </summary>
    [RequireComponent(typeof(MeshRenderer))]
    public class MaterialHighlightable : MonoBehaviour
    {
        [field: SerializeField] public Material InitialMaterial { get; private set; }
        [field: SerializeField] public Material CurrentDefaultMaterial { get; private set; }
        [field: SerializeField] public MeshRenderer TargetRenderer { get; private set; }

        private void Awake()
        {
            if (!TargetRenderer)
            {
                Debug.LogError($"{nameof(TargetRenderer)} is not set");
            }
        }

        /// <summary>
        ///     Meant to be used only once during set up, sets the initial material of the element.
        /// </summary>
        /// <param name="initialMaterial">The initial material of the element that should not change after.</param>
        public void SetupInitialMaterial(Material initialMaterial)
        {
            InitialMaterial = initialMaterial;
            TargetRenderer.sharedMaterial = initialMaterial;
        }

        /// <summary>
        ///     Change the current default material to the one given.
        ///     The current default material is the material set when the element is not highlighted.
        /// </summary>
        /// <param name="newDefaultMaterial">The new default material of the element.</param>
        public void UpdateCurrentDefaultMaterial(Material newDefaultMaterial)
        {
            CurrentDefaultMaterial = newDefaultMaterial;
        }

        /// <summary>
        ///     Updates the current element's renderer material to highlight the element.
        /// </summary>
        /// <param name="highlightMaterial">The material to set.</param>
        public void Highlight(Material highlightMaterial)
        {
            if (TargetRenderer)
                TargetRenderer.material = highlightMaterial;
        }

        /// <summary>
        ///     Resets the element's renderer material to the current default material of the element.
        /// </summary>
        public void ResetToDefaultMaterial()
        {
            if (TargetRenderer)
                TargetRenderer.material = CurrentDefaultMaterial;
        }

        /// <summary>
        ///     Resets the element's renderer material to the initial material of the element.
        /// </summary>
        public void ResetToInitialMaterial()
        {
            if (TargetRenderer)
                TargetRenderer.material = InitialMaterial;
            CurrentDefaultMaterial = InitialMaterial;
        }
    }
}