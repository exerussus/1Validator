using UnityEngine;

namespace Exerussus._1Validator
{
    public static class Extensions
    {
        public static string GetPathToComponent(this Component monoBeh)
        {
            var path = "";
            var maxCount = 50;
            var type = monoBeh.GetType();

            GetPathFrom(monoBeh.transform, type, ref path, maxCount);
            path += monoBeh.transform.gameObject.name + "/";
            path += type.Name;
            return path;
        }

        private static void GetPathFrom(Transform transform, System.Type componentType,ref string path, int countRemaining)
        {
            if (countRemaining <= 0 || transform.parent == null) return;
            
            path += transform.parent.name + "/";
            GetPathFrom(transform.parent, componentType, ref path, countRemaining - 1);
        }
        
        public static string GetPrefabPath(this Component monoBehaviour)
        {
#if UNITY_EDITOR
            GameObject prefabRoot = monoBehaviour.transform.root.gameObject;
            if (prefabRoot.scene.name == null) return UnityEditor.AssetDatabase.GetAssetPath(prefabRoot);
#endif

                
            return null;
        }
        
        public static string GetFullPath(this Component component)
        {
            var prefabPath = component.GetPrefabPath();
            var path = component.GetPathToComponent();

            var result = "";
            if (prefabPath != null) result += $"Prefab path: {prefabPath}\n";
            result += $"script path: {path}\n";
            return result;
        }
    }
}