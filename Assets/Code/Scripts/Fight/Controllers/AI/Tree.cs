using UnityEngine;

namespace FrostfallSaga.Fight.Assets.Code.Scripts.Fight.Controllers.AI
{
    public abstract class Tree : MonoBehaviour
    {
        private Node _node = null;

        protected void Start()
        {
            _node = SetupTree();
        }

        private void Update()
        {
            if (_node != null)
            {
                _node.Evaluate();
            }
        }

        protected abstract Node SetupTree();
    }
}