using UnityEngine;

namespace FrostfallSaga.Grid.Cells
{
    /// <summary>
    /// Responsible for managing the visual aspects of the cell.
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    public class CellVisual : MonoBehaviour
    {
        [field: SerializeField] public Material DefaultMaterial { get; private set; }

        /// <summary>
        /// Change the default material to the one given.
        /// The default material is the material set when the cell is not highlighted.
        /// </summary>
        /// <param name="newDefaultMaterial">The new default material of the cell.</param>
        public void UpdateDefaultMaterial(Material newDefaultMaterial)
        {
            DefaultMaterial = newDefaultMaterial;
        }

        /// <summary>
        /// Updates the current cell's renderer material to highlight the cell.
        /// </summary>
        /// <param name="highlightMaterial">The material to set.</param>
        public void Highlight(Material highlightMaterial)
        {
            GetComponent<Renderer>().material = highlightMaterial;
        }

        /// <summary>
        /// Resets the cell's renderer material to the default material of the cell.
        /// </summary>
        public void ResetMaterial()
        {
            GetComponent<Renderer>().material = DefaultMaterial;
        }
    }
}
