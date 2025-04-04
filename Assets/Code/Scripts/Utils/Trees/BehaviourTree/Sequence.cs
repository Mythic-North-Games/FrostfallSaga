using System.Collections.Generic;

namespace FrostfallSaga.Utils.Trees.BehaviourTree
{
    /// <summary>
    ///     Acts like a logical AND.
    ///     If any child node fails, the sequence will fail and the other child branches will not be evaluated.
    /// </summary>
    public class Sequence : Node
    {
        public Sequence()
        {
        }

        public Sequence(List<Node> children) : base(children)
        {
        }

        public override NodeState Evaluate()
        {
            bool anyChildIsRunning = false;

            foreach (Node node in children)
                switch (node.Evaluate())
                {
                    case NodeState.FAILURE:
                        state = NodeState.FAILURE;
                        return state;
                    case NodeState.SUCCESS:
                        continue;
                    case NodeState.RUNNING:
                        anyChildIsRunning = true;
                        continue;
                    default:
                        state = NodeState.SUCCESS;
                        return state;
                }

            state = anyChildIsRunning ? NodeState.RUNNING : NodeState.SUCCESS;
            return state;
        }
    }
}