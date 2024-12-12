using System;

namespace FrostfallSaga.Utils
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class RequiredAttribute : Attribute {}
}
