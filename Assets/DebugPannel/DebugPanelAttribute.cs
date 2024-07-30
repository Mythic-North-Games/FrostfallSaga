using System;

namespace FrostfallSaga.DebugPanel
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class DebugPanelAttribute : Attribute
    {
        public string Description { get; }
        public string Category { get; }

        public DebugPanelAttribute(string info, string categoryName)
        {
            Description = info;
            Category = categoryName;
        }

    }
}