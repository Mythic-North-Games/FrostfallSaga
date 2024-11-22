using UnityEditor;
using FrostfallSaga.Editors;
using FrostfallSaga.Fight.Abilities.AbilityAnimation;

namespace FrostfallSaga.FFSEditor.GameObjectVisuals
{
    [CustomPropertyDrawer(typeof(AInternalAbilityAnimationExecutor), true)]
    public class AInternalAbilityAnimationExecutorDrawer : AbstractAttributeDrawer<AInternalAbilityAnimationExecutor>
    {
    }
}
