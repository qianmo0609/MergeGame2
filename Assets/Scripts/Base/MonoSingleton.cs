using UnityEngine;
public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static object _lock = new object();

    private static T _instance;
    public static T Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                return null;
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));
                    if (_instance == null)
                    {
                        GameObject singleton = new GameObject("M_" + typeof(T).Name);
                        _instance = singleton.AddComponent<T>();
                        DontDestroyOnLoad(singleton);
                    }
                }
                return _instance;
            }
        }
    }
  
    public virtual void OnInit() { }


    private static bool applicationIsQuitting = false;

    public virtual void OnDestroy()
    {
        applicationIsQuitting = true;
    }
}
