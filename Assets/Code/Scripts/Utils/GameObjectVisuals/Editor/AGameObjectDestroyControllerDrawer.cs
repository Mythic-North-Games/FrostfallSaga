using FrostfallSaga.Utils.Editor;
using FrostfallSaga.Utils.GameObjectVisuals.GameObjectDestroyControllers;
using UnityEditor;

namespace FrostfallSaga.FFSEditor.GameObjectVisuals
{
    [CustomPropertyDrawer(typeof(AGameObjectDestroyController), true)]
    public class AGameObjectDestroyControllerDrawer : AbstractAttributeDrawer<AGameObjectDestroyController>
    {
    }
}