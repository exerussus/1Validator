using System;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Exerussus._1Validator.Validators
{
    public class GetComponentInChildren : IFieldValidator
    {
        public Type AttributeType => typeof(ValidateGetInChildrenComponentAttribute);
        
        public void ValidateField(Component component, FieldInfo field, Validator.Result result)
        {
            var fieldValue = field.GetValue(component) as Component;
            if (fieldValue == null)
            {
                var newComponent = component.gameObject.GetComponentInChildren(field.FieldType);
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
    public class ValidateGetInChildrenComponentAttribute : Attribute
    {
        public ValidateGetInChildrenComponentAttribute() {}
    }
}