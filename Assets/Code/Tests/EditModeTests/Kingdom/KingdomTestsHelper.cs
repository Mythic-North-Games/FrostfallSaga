using FrostfallSaga.Kingdom;
using FrostfallSaga.Kingdom.EntitiesGroups;
using UnityEngine;

namespace FrostfallSaga.EditModeTests.Kingdom
{
    public static class KingdomTestsHelper
    {
        public static EntitiesGroup CreateEntitiesGroup(KingdomCell currentCell, int movePoints = 3)
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