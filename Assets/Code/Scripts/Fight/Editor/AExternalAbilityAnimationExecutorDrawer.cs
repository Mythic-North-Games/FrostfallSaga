using UnityEditor;
using FrostfallSaga.Editors;
using FrostfallSaga.Fight.Abilities.AbilityAnimation;

namespace FrostfallSaga.FFSEditor.GameObjectVisuals
{
    [CustomPropertyDrawer(typeof(AExternalAbilityAnimationExecutor), true)]
    public class AExternalAbilityAnimationExecutorDrawer : AbstractAttributeDrawer<AExternalAbilityAnimationExecutor>
    {
    }
}
