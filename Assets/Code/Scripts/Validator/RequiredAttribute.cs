using System;

namespace FrostfallSaga.Validator
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class RequiredAttribute : Attribute {}
}
