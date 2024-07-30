using System;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
sealed class DebugPanelAttribute : Attribute
{
    public string Description { get; }
    public string Category { get; }

    public DebugPanelAttribute(string info, string categoryName)
    {
        Description = info;
        Category = categoryName;
    }
}