using UnityEngine;
using FrostfallSaga.Grid.Cells;

namespace FrostfallSaga.EntitiesVisual
{
    public class EntityVisualMovementController : MonoBehaviour
    {
        private Vector3 _targetCellPosition;
        
        public void  Move(Cell cell)
        {
            _targetCellPosition = cell.GetCenter();
            transform.LookAt(cell.GetCenter());
        }

        private void Update() {
            if ( _targetCellPosition == null){
                return;
            }
            transform.position =  Vector3.Lerp(transform.position, _targetCellPosition, speed* Time.deltaTime); 
        }
    }
}