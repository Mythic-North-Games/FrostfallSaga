using UnityEditor;
using FrostfallSaga.Editors;
using FrostfallSaga.GameObjectVisuals.GameObjectSpawnControllers;

namespace FrostfallSaga.FFSEditor.GameObjectVisuals
{
    [CustomPropertyDrawer(typeof(AGameObjectSpawnController), true)]
    public class AGameObjectSpawnControllerDrawer : AbstractAttributeDrawer<AGameObjectSpawnController>
    {
    }
}
