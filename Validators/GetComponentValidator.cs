using System;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Exerussus._1Validator.Validators
{
    public class GetComponentValidator : IFieldValidator
    {
        public Type AttributeType => typeof(ValidateGetComponentAttribute);
        
        public void ValidateField(Component component, FieldInfo field, Validator.Result result)
        {
            var fieldValue = field.GetValue(component) as Component;
            if (fieldValue == null)
            {
                var newComponent = component.gameObject.GetComponent(field.FieldType);
                if (newComponent != null)
                {
                    field.SetValue(component, newComponent);
                    result.Changed = true;
                }
                else Debug.LogWarning($"Не удалось найти компонент {field.FieldType}!\n{component.GetFullPath()}field : {field.Name}\n", component);
            }
        }
    }

    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class ValidateGetComponentAttribute : Attribute
    {
        public ValidateGetComponentAttribute() {}
    }
}