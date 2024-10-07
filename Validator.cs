#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;


namespace Exerussus._1Validator
{
    public static class Validator
    {
        private static List<IFieldValidator> _fieldValidators = new();
        
        [MenuItem("Tools/Exerussus/Validate")]
        public static void Validate()
        {
            SetValidators(ref _fieldValidators);
            ValidateFields();
        }

        #region Base

        private static void SetValidators<T>(ref List<T> validators) where T : class, IValidator
        {
            var types = Assembly.GetExecutingAssembly().GetTypes().Where(t =>
                t.GetInterfaces().Contains(typeof(T)) && !t.IsAbstract && !t.IsInterface).ToList();
            validators.Clear();
            foreach (var type in types) validators.Add(Activator.CreateInstance(type) as T);
        }
        
        public class Result
        {
            public bool Changed;
        }

        #endregion

        #region Field Validation

        private static void ValidateFields()
        {
            if (_fieldValidators.Count == 0) return;
            
            ValidateFieldsGameObjects();
            ValidateFieldsPrefabs();
            UnityEditor.AssetDatabase.SaveAssets();
        }

        private static void ValidateFieldsGameObjects()
        {
            var allGameObjects = Object.FindObjectsOfType<GameObject>(true);

            foreach (var gameObject in allGameObjects)
            {
                var components = gameObject.GetComponents<MonoBehaviour>();
                foreach (var component in components)
                {
                    InvokeValidation(component);
                    foreach (var fieldValidator in _fieldValidators) ValidateComponentFields(component, fieldValidator.AttributeType, fieldValidator.ValidateField, new Result());
                }
            }
        }

        private static void ValidateFieldsPrefabs()
        {
            var prefabs = UnityEditor.AssetDatabase.FindAssets("t:Prefab")
                .Select(guid => UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(UnityEditor.AssetDatabase.GUIDToAssetPath(guid)))
                .ToList();

            foreach (var prefab in prefabs)
            {
                var monoBehs = prefab.GetComponentsInChildren<MonoBehaviour>();
                var methodExist = false;
                
                foreach (var monoBeh in monoBehs)
                {
                    var fieldResult = new Result();
                    if (InvokeValidation(monoBeh)) methodExist = true;
                    foreach (var fieldValidator in _fieldValidators) ValidateComponentFields(monoBeh, fieldValidator.AttributeType, fieldValidator.ValidateField, fieldResult);
                    
                    if (fieldResult.Changed) UnityEditor.EditorUtility.SetDirty(prefab);
                }
                
                if (methodExist) UnityEditor.EditorUtility.SetDirty(prefab);
            }
        }

        private static void ValidateComponentFields(MonoBehaviour component, Type attributeType, Action<Component,  FieldInfo, Result> actionWithField, Result result)
        {
            var fields = component.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
    
            foreach (var field in fields)
            {
                if (Attribute.IsDefined(field, attributeType))
                {
                    if (typeof(Component).IsAssignableFrom(field.FieldType))
                    {
                        actionWithField(component, field, result);
                    }
                    else Debug.LogError($"Поле не является компонентом!\n{component.GetFullPath()}field : {field.Name}\n", component);
                }
            }
        }

        private static bool InvokeValidation(MonoBehaviour component)
        {
            var methods = component.GetType().GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var executed = false;
            
            foreach (var method in methods)
            {
                if (Attribute.IsDefined(method, typeof(ValidationMethodAttribute)))
                {
                    method.Invoke(component, null);
                    executed = true;
                }
            }

            return executed;
        }

        #endregion

    }
    
#region Interfaces

    public interface IValidator
    {
        public Type AttributeType { get; }
    }
    
    public interface IFieldValidator: IValidator
    {
        public void ValidateField(Component component, FieldInfo field, Validator.Result result);
    }
    
#endregion
    
#endif
}