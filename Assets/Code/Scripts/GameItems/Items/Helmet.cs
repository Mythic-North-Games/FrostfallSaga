using System.Collections;
using System.Collections.Generic;
using FrostfallSaga.Core;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Item/Helmet")]
public class Helmet  : Item
{
    public int Level;
    public int MinDamage;
    public int MaxDamage;
    public List<EntityID> WeaknessList;
    public List<EntityID> StrenghtList;
    public Helmet()
    {
        ItemTag = SlotTag.Head;
    }
}