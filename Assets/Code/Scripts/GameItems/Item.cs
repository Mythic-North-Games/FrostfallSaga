using System.Collections;
using System.Collections.Generic;
using FrostfallSaga.Core;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Item/Item")]
public class Item : ScriptableObject
{
    public Sprite SpriteTexture;
    public SlotTag ItemTag;
    public RarityTag Rarity;
    public string Name;
    public string Description;
    public int BasePrice;
}