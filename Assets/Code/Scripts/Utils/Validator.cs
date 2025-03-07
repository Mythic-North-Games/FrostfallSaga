using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace FrostfallSaga.Utils
{
    public static class Validator
    {
        public static void ValidateRequiredFields(object target)
        {
            Type type = target.GetType();
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (FieldInfo field in fields)
            {
                var requiredAttr = field.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();

                if (requiredAttr != null)
                {
                    var value = field.GetValue(target);
                    if (value == null) Debug.LogError($"Required field '{field.Name}' is not set in {type.Name}.");
                }
            }
        }
    }
}