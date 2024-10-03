using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrostfallSaga.Fight.Assets.Code.Scripts.Fight.Controllers.AI
{
    public class Selector : Node
    {
        public Selector() : base { }
        public Selector(List<Node> children) : base(children) { }

        public override NodeState Evaluate()
        {
            foreach (Node node in children)
            {
                switch (node.Evaluate)
                {
                    case NodeState.FAILURE:
                        continue;
                    case NodeState.SUCCESS:
                        return NodeState.SUCCESS;
                    case NodeState.RUNNING:
                        return NodeState.RUNNING;
                    default:
                        continue;
                }
            }
            return NodeState.FAILURE;
        }
    }
