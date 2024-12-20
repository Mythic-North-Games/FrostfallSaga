using UnityEditor;
using FrostfallSaga.Utils.Editor;
using FrostfallSaga.Utils.GameObjectVisuals.GameObjectDestroyControllers;

namespace FrostfallSaga.FFSEditor.GameObjectVisuals
{
    [CustomPropertyDrawer(typeof(AGameObjectDestroyController), true)]
    public class AGameObjectDestroyControllerDrawer : AbstractAttributeDrawer<AGameObjectDestroyController>
    {
    }
}
