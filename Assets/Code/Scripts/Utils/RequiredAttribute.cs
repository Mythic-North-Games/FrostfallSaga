using System;

namespace FrostfallSaga.Utils
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class RequiredAttribute : Attribute
    {
    }
}