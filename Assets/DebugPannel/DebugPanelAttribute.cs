using System;

namespace FrostfallSaga.DebugPanel
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class DebugPanelAttribute : Attribute
    {
        public DebugPanelAttribute(string info, string categoryName)
        {
            Description = info;
            Category = categoryName;
        }

        public string Description { get; }
        public string Category { get; }
    }
}