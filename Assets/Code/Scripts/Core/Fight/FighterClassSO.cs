using System.Collections.Generic;
using UnityEngine;
using FrostfallSaga.Utils;
using FrostfallSaga.Utils.DataStructures.GraphNode;

namespace FrostfallSaga.Core.Fight
{
    [CreateAssetMenu(fileName = "FighterClass", menuName = "ScriptableObjects/Fight/FighterClass", order = 0)]
    public class FighterClassSO : ScriptableObject
    {
        [field: SerializeField] public string ClassName { get; private set; }
        [field: SerializeField] public int ClassMaxHealth { get; private set; }
        [field: SerializeField] public int ClassMaxActionPoints { get; private set; }
        [field: SerializeField] public int ClassMaxMovePoints { get; private set; }
        [field: SerializeField] public int ClassStrength { get; private set; }
        [field: SerializeField] public int ClassDexterity { get; private set; }
        [field: SerializeField] public int ClassTenacity { get; private set; }
        [field: SerializeField] public int ClassPhysicalResistance { get; private set; }

        [field: SerializeField] private SElementToValue<EMagicalElement, int>[] _classMagicalResistances;
        public Dictionary<EMagicalElement, int> ClassMagicalResistances
        {
            get => SElementToValue<EMagicalElement, int>.GetDictionaryFromArray(_classMagicalResistances);
            private set => _classMagicalResistances = SElementToValue<EMagicalElement, int>.GetArrayFromDictionary(value);
        }

        [field: SerializeField] private SElementToValue<EMagicalElement, int>[] _classMagicalStrengths;
        public Dictionary<EMagicalElement, int> ClassMagicalStrengths
        {
            get => SElementToValue<EMagicalElement, int>.GetDictionaryFromArray(_classMagicalStrengths);
            private set => _classMagicalStrengths = SElementToValue<EMagicalElement, int>.GetArrayFromDictionary(value);
        }

        [field: SerializeField] public float ClassDodgeChance { get; private set; }
        [field: SerializeField] public float ClassMasterstrokeChance { get; private set; }
        [field: SerializeField] public int ClassInitiative { get; private set; }
        [field: SerializeField] public ClassGodSO God { get; private set; }

        //////////////////::
        /// Ability tree ///
        //////////////////::

        [SerializeField] private List<GraphNodeDTO<ABaseAbility>> _abilityNodes = new(); // Serialized data
        private Dictionary<string, GraphNode<ABaseAbility>> _runtimeGraph = new(); // Runtime-only map

        public GraphNode<ABaseAbility> AbilitiesGraphRoot => _runtimeGraph.Count > 0 ? FindRoot() : null;

        private GraphNode<ABaseAbility> FindRoot()
        {
            foreach (var node in _runtimeGraph.Values)
            {
                if (node.Parents.Count == 0) return node;
            }
            return null;
        }

        private void OnEnable()
        {
            RebuildRuntimeGraph();
        }

        public void RebuildRuntimeGraph()
        {
            var nodes = GraphNodeSerializer.DeserializeGraph(_abilityNodes);
            _runtimeGraph.Clear();
            foreach (var node in nodes)
            {
                _runtimeGraph[node.ID] = node;
            }
        }

        public void SaveRuntimeGraph()
        {
            _abilityNodes = GraphNodeSerializer.SerializeGraph(new List<GraphNode<ABaseAbility>>(_runtimeGraph.Values));
        }

        public Dictionary<string, GraphNode<ABaseAbility>> GetGraphMap() => _runtimeGraph;
    }
}
