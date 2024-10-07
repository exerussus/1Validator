﻿

namespace Exerussus._1Validator.Validators
{
    
#if UNITY_EDITOR
    public class GetComponentValidator : IFieldValidator
    {
        public System.Type AttributeType => typeof(ValidateGetComponentAttribute);
        
        public void ValidateField(UnityEngine.Component component, System.Reflection.FieldInfo field, Validator.Result result)
        {
            var fieldValue = field.GetValue(component) as UnityEngine.Component;
            if (fieldValue == null)
            {
                var newComponent = component.gameObject.GetComponent(field.FieldType);
                if (newComponent != null)
                {
                    field.SetValue(component, newComponent);
                    result.Changed = true;
                }
                else UnityEngine.Debug.LogWarning($"Не удалось найти компонент {field.FieldType}!\n{component.GetFullPath()}field : {field.Name}\n", component);
            }
        }
    }
#endif
    
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    [System.AttributeUsage(System.AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class ValidateGetComponentAttribute : System.Attribute
    {
        public ValidateGetComponentAttribute() {}
    }
    
}
