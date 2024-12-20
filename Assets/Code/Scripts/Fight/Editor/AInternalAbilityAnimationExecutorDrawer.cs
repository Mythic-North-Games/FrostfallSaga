using UnityEditor;
using FrostfallSaga.Utils.Editor;
using FrostfallSaga.Fight.Abilities.AbilityAnimation;

namespace FrostfallSaga.FFSEditor.GameObjectVisuals
{
    [CustomPropertyDrawer(typeof(AInternalAbilityAnimationExecutor), true)]
    public class AInternalAbilityAnimationExecutorDrawer : AbstractAttributeDrawer<AInternalAbilityAnimationExecutor>
    {
    }
}
