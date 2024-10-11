using FrostfallSaga.Fight.Effects;
using System.Collections;
using System.Collections.Generic;
using FrostfallSaga.Fight.Fighters;
using UnityEngine;

namespace FrostfallSaga.Fight.Fighters {
    [CreateAssetMenu(fileName = "FighterClass", menuName = "ScriptableObjects/Fight/FighterClass", order = 0)]
    public class FighterClassSO : ScriptableObject {
        public int classMaxHealth;
        public int classMaxActionPoints;
        public int classMaxMovePoints;
        public int classStrength;
        public int classDexterity;
        public float classTenacity;
        public float classDodge;
        public int classPhysicalResistance;
        public MagicalElementToValue[] classMagicalResistances;
        public MagicalElementToValue[] classMagicalStrengths;
        public float classMasterstroke;
        public int classInitiative;

    }
}