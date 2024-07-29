using System;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
sealed class DebugPanelAttribute : Attribute
{
    public string Description { get; }

    public DebugPanelAttribute(string info)
    {
        Description = info;
    }
}