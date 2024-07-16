using System;
using System.Collections.Generic;
using FrostfallSaga.Kingdom.Entities;

namespace FrostfallSaga.Kingdom.EntitiesGroups
{
    /// <summary>
    ///	Represents a group of enemies in a kingdom.
    /// </summary>
    public class EnemiesGroup : EntitiesGroup
    {
        private void Start()
        {

        }

        public void UpdateEntities(Entity[] newMembers)
        {
            Entities = new List<Entity>(newMembers).ToArray();
            
            for (int i = 0; i < Entities.Length; i++)
            {
                if (i == 0)
                {
                    UpdateDisplayedEntity(Entities[i]);
                }
                else
                {
                    Entities[i].HideVisual();
                }

                Entities[i].name = Enum.GetName(typeof(EntityID), Entities[i].EntityConfiguration.EntityID) + i;
                Entities[i].transform.parent = transform;
                Entities[i].transform.localPosition = new(0, 0, 0);
            }
            
        }
    }
}