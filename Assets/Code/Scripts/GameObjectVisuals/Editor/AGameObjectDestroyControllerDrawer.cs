using UnityEditor;
using FrostfallSaga.Editors;
using FrostfallSaga.GameObjectVisuals.GameObjectDestroyControllers;

namespace FrostfallSaga.FFSEditor.GameObjectVisuals
{
    [CustomPropertyDrawer(typeof(AGameObjectDestroyController), true)]
    public class AGameObjectDestroyControllerDrawer : AbstractAttributeDrawer<AGameObjectDestroyController>
    {
    }
}
