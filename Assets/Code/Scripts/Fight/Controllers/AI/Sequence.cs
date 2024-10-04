using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrostfallSaga.Fight.Assets.Code.Scripts.Fight.Controllers.AI
{
    public class Sequence : Node
    {
        public Sequence() { }
        public Sequence(List<Node>) : base(children) { }

        public override NodeState Evaluate()
        {
            bool anyChildIsRunning = false;

            foreach (Node node in children)
            {
                switch (node.Evaluate)
                {
                    case NodeState.FAILURE:
                        return NodeState.FAILURE;
                    case NodeState.SUCCESS:
                        continue;
                    case NodeState.RUNNING:
                        anyChildIsRunning = true;
                        continue;
                    default:
                        return NodeState.SUCCESS;
                }

                return anyChildIsRunning ? NodeState.RUNNING : NodeState.SUCCESS;  
            } 
        }
    }
}
