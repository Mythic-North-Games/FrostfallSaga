using System.Collections;
using System.Collections.Generic;
using FrostfallSaga.Core;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Item/Chestplate")]
public class Chestplate  : Item
{
    public int Level;
    public int MinDamage;
    public int MaxDamage;
    public List<EntityID> WeaknessList;
    public List<EntityID> StrenghtList;

    public Chestplate()
    {
        ItemTag = SlotTag.Chest;
    }
}