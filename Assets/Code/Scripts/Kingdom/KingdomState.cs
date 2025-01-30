using System.Collections.Generic;
using UnityEngine;
using FrostfallSaga.Utils;
using FrostfallSaga.Kingdom.EntitiesGroups;
using FrostfallSaga.Kingdom.CityBuildings;

namespace FrostfallSaga.Kingdom
{
    public class KingdomState : MonoBehaviourPersistingSingleton<KingdomState>
    {
        public EntitiesGroupData heroGroupData;
        public EntitiesGroupData[] enemiesGroupsData;
        public CityBuildingData[] cityBuildingsData;

        public void SaveKingdomData(EntitiesGroup heroGroup, List<EntitiesGroup> enemiesGroups, CityBuilding[] cityBuildings)
        {
            // Extract & save hero group data
            heroGroupData = EntitiesGroupBuilder.Instance.ExtractEntitiesGroupDataFromEntiesGroup(heroGroup);

            // Extract & save enemies groups data
            enemiesGroupsData = new EntitiesGroupData[enemiesGroups.Count];
            for (int i = 0; i < enemiesGroups.Count; i++)
            {
                enemiesGroupsData[i] = EntitiesGroupBuilder.Instance.ExtractEntitiesGroupDataFromEntiesGroup(enemiesGroups[i]);
            }

            // Extract & save city buildings data
            cityBuildingsData = new CityBuildingData[cityBuildings.Length];
            for (int i = 0; i < cityBuildings.Length; i++)
            {
                cityBuildingsData[i] = new CityBuildingData(cityBuildings[i]);
            }

            Debug.Log("KingdomConfiguration Saved !");
        }
    }
}

