using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrostfallSaga.Utils.GameObjectVisuals;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.FightCells;
using System.Linq;

namespace FrostfallSaga.Fight.Abilities.AbilityAnimation
{

    [Serializable]
    public class StaticObjectsExecutor : AExternalAbilityAnimationExecutor
    {
       
        public override void Execute(Fighter fighterThatExecutes, FightCell[] abilityCells, GameObject objectPrefab)
        {

            foreach (FightCell targetCell in abilityCells)
            {
                GameObject projectile = UnityEngine.Object.Instantiate(objectPrefab, targetCell.transform);
            }

        }


    }
}
