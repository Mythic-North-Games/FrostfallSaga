using UnityEngine.UIElements;

namespace FrostfallSaga.City.UI
{
    public class LeftContainerController
    {
        private static readonly string LEFT_CONTAINER_HIDDEN_CLASSNAME = "leftContainerHidden";

        private readonly VisualElement _containerRoot;

        public LeftContainerController(VisualElement leftContainerRoot)
        {
            _containerRoot = leftContainerRoot;
        }

        public void Display()
        {
            _containerRoot.RemoveFromClassList(LEFT_CONTAINER_HIDDEN_CLASSNAME);
        }

        public void Hide()
        {
            _containerRoot.AddToClassList(LEFT_CONTAINER_HIDDEN_CLASSNAME);
        }
    }
}