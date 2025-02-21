using UnityEditor;
using FrostfallSaga.Core.Quests;
using FrostfallSaga.Utils.Editor;

namespace FrostfallSaga.FFSEditor.GameObjectVisuals
{
    [CustomPropertyDrawer(typeof(AQuestAction), true)]
    public class AExternalAbilityAnimationExecutorDrawer : AbstractAttributeDrawer<AQuestAction>
    {
    }
}
