using UnityEngine;

namespace Workshop.Scaffolding.Nature.Scripts.Utils
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T instance;

        public static bool HasInstance => instance != null;
    
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<T>(FindObjectsInactive.Include);
                    if (instance == null)
                    {
                        var obj = new GameObject();
                        instance = obj.AddComponent<T>();
                        instance.name = $"[Singleton][{typeof(T).Name}]";
                    
                        Debug.Log($"[Singleton] New {typeof(T).Name} singleton instantiated.");
                    }
                }
                return instance;
            }
        }
    
        protected virtual void Awake()
        {
            if (!Application.isPlaying)
            {
                return;
            }
            // Prevents `_instance` corruption.
            if (instance != null && instance != this)
            {
                return;
            }
            instance = this as T;			
        }
    }
}