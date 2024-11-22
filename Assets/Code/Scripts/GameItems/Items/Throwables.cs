using System.Collections;
using System.Collections.Generic;
using FrostfallSaga.Fight.Effects;
using FrostfallSaga.Core;
using Random = System.Random;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Item/Thowable")]
public class Throwable  : Item
{
    public int Level;
    public int ammount;
    public bool isInfinite;
    public int MinDamage;
    public int MaxDamage;
    public List<EntityID> WeaknessList;
    public List<EntityID> StrenghtList;
    public List<AEffectSO> Effects;
    public Throwable()
    {
        ItemTag = SlotTag.Secondary;
    }
    public int GetRawDamage()
    {
        if(MinDamage != 0 && MaxDamage != 0)
        {
            Random rnd = new Random();
            return rnd.Next(MinDamage, MaxDamage); // Si les deux sont définis on renvoie un nb random dans la plage
        }
        else if(MinDamage == 0 && MaxDamage == 0)
        {
            return 0; // Si les deux sont indéfinis on renvoie 0
        }
        else if (MinDamage != 0){
            return MinDamage; // Si seulement min on renvoie min
        }
        else if (MaxDamage != 0){
            Random rnd = new Random();
            return rnd.Next(0, MaxDamage); // Si seulement max on renvoie un nb random entre 0 et max
        }
        return 0;
    }

    public int GetComputedDamages(EntityID TargetCategory)
    {
        int rawDamage = GetRawDamage();
        if(WeaknessList.Contains(TargetCategory))
        {
            rawDamage = (int) (0.8 * rawDamage); // Si dans la liste on remove 20% de ses damages
        }
        if(StrenghtList.Contains(TargetCategory))
        {
            rawDamage =  (int) (1.2 * rawDamage); // Si dans la liste on add 20% de ses damages
        }
        return rawDamage;
    }
}