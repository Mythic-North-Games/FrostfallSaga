using FrostfallSaga.Core.GameState;
using FrostfallSaga.Core.GameState.Fight;
using FrostfallSaga.Dungeon;
using FrostfallSaga.Fight.FightCells;
using FrostfallSaga.Grid;
using FrostfallSaga.Utils;
using UnityEngine;

namespace FrostfallSaga.Fight
{
    public class FightHexGrid : AHexGrid
    {
        [SerializeField] private BiomeTypeSO defaultBiomeType;
        private DungeonGridGenerator _dungeonGridGenerator;
        private FightGridGenerator _fightGridGenerator;
        private FightCell _hexFightPrefab;

        public override void GenerateGrid()
        {
            if (Cells.Count > 0)
                ClearCells();
            EFightOrigin fightOrigin = GameStateManager.Instance.GetPreFightData().fightOrigin;
            Seed = Randomizer.GetRandomIntBetween(000_000_000, 999_999_999);
            Debug.Log($"Fight origin : {fightOrigin}");
            switch (fightOrigin)
            {
                case EFightOrigin.KINGDOM:
                    _fightGridGenerator = new FightGridGenerator(_hexFightPrefab, Width, Height, AvailableBiomes,
                        transform,
                        NoiseScale, Seed, defaultBiomeType);
                    Cells = _fightGridGenerator.GenerateGrid();
                    break;
                case EFightOrigin.DUNGEON:
                    _dungeonGridGenerator = new DungeonGridGenerator(_hexFightPrefab, Width, Height,
                        transform, NoiseScale, Seed, defaultBiomeType);
                    Cells = _dungeonGridGenerator.GenerateGrid();
                    break;
                default:
                    Debug.Log($"{fightOrigin.ToString()} is not a valid Fight Origin.");
                    return;
            }
        }

        #region Setup & tear down

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            _hexFightPrefab = Resources.Load<FightCell>("Prefabs/Grid/FightCell");
            if (!_hexFightPrefab) Debug.LogError("FightCellPrefab is null.");
            defaultBiomeType ??= Resources.Load<BiomeTypeSO>("ScriptableObjects/Grid/Biome/BiomeTypeValley");
        }

        #endregion
    }
}