﻿using UnityEngine;

namespace FrostfallSaga.Fight.Fighters
{
    [CreateAssetMenu(fileName = "FighterClass", menuName = "ScriptableObjects/Fight/FighterClass", order = 0)]
    public class FighterClassSO : ScriptableObject
    {
        public int classMaxHealth;
        public int classMaxActionPoints;
        public int classMaxMovePoints;
        public int classStrength;
        public int classDexterity;
        public float classTenacity;
        public int classPhysicalResistance;
        public MagicalElementToValue[] classMagicalResistances;
        public MagicalElementToValue[] classMagicalStrengths;
        public float classDodgeChance;
        public float classMasterstrokeChance;
        public int classInitiative;
    }
}