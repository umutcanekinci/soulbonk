using UnityEngine;

namespace VectorViolet.Core.Utilities
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        public static T Instance { get; private set; }
        protected virtual bool DontDestroy => true;

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;

                if (transform.parent != null)
                {
                    transform.SetParent(null);
                }

                if (DontDestroy)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}