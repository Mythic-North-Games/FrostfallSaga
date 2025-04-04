using FrostfallSaga.Core.Quests;
using FrostfallSaga.Utils.Editor;
using UnityEditor;

namespace FrostfallSaga.FFSEditor.GameObjectVisuals
{
    [CustomPropertyDrawer(typeof(AQuestAction), true)]
    public class AExternalAbilityAnimationExecutorDrawer : AbstractAttributeDrawer<AQuestAction>
    {
    }
}