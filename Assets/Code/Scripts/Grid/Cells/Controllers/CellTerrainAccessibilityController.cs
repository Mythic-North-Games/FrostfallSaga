using FrostfallSaga.Utils;

namespace FrostfallSaga.Grid.Cells
{
    public class CellTerrainAccessibilityController
    {
        private readonly TerrainTypeSO _terrainType;
        private bool? _accessibilityOverride;
        public bool IsAccessible => _accessibilityOverride ?? _terrainType.DefaultAccessible;


        public CellTerrainAccessibilityController(TerrainTypeSO terrainType)
        {
            _terrainType = terrainType;
        }
        
        public void GenerateRandomAccessibility(EAccessibilityGenerationMode mode)
        {
            bool defaultValue = _terrainType.DefaultAccessible;
            float chance = _terrainType.AccessibilityChanceOverride;

            switch (mode)
            {
                case EAccessibilityGenerationMode.NONE:
                    _accessibilityOverride = null;
                    break;

                case EAccessibilityGenerationMode.FLIP_IF_ACCESSIBLE:
                    _accessibilityOverride = defaultValue && Randomizer.GetBooleanOnChance(chance);
                    break;

                case EAccessibilityGenerationMode.FLIP_IF_NOT_ACCESSIBLE:
                    _accessibilityOverride = !defaultValue && Randomizer.GetBooleanOnChance(chance) || defaultValue;
                    break;

                case EAccessibilityGenerationMode.BIDIRECTIONAL_FLIP:
                    bool shouldFlip = Randomizer.GetBooleanOnChance(chance);
                    _accessibilityOverride = shouldFlip ? !defaultValue : defaultValue;
                    break;
            }
        }
        
        public void OverrideAccessibility(bool value) => _accessibilityOverride = value;
        
        
        public void ResetToDefault() => _accessibilityOverride = null;

        public override string ToString()
        {
            return $"CellAccessibility: IsAccessible={IsAccessible} (Overridden={_accessibilityOverride.HasValue})";
        }
    }
}