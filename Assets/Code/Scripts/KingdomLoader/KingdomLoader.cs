using System;
using UnityEngine;

namespace FrostfallSaga.kingdomeLoader
{
    [field: SerializeField] public KingdomData kindgomData;
    [field: SerializeField] public FightData fightData;
    [field: SerializeField] public Cell heroGroupStartCell;
    public UnityEvent<KingdomLoadContext> onKingdomLoaded;


    private void Start() {
        LoadKingdom();
    }
    private void LoadKingdom(){
        
    }


}
