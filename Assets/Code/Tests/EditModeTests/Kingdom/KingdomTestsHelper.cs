using UnityEngine;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Kingdom.EntitiesGroups;

namespace FrostfallSaga.EditModeTests.Kingdom
{
    public static class KingdomTestsHelper
    {
        public static EntitiesGroup CreateEntitiesGroup(Cell currentCell, int movePoints = 3)
        {
            GameObject entitiesGroupGameObject = new();
			entitiesGroupGameObject.AddComponent<EntitiesGroup>();
			EntitiesGroup entitiesGroup = entitiesGroupGameObject.GetComponent<EntitiesGroup>();

            entitiesGroup.cell = currentCell;
			entitiesGroup.movePoints = movePoints;
            
			return entitiesGroup;
        }
	}
}