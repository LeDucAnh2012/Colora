using UnityEngine;

public enum TypeMode
{
    Release,
    Development,
    Marketing
}

public class KeepObject : MonoBehaviour
{
    public static KeepObject instance;
    public bool isHack = false;
    public TypeMode mode;

    private static bool isReady = false;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            isReady = true;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (!isReady)
                Destroy(gameObject);
        }

        Application.targetFrameRate = 120;
        Vibration.Init();
    }
}
